#pragma warning disable OPENAI001 // VectorStore APIs are experimental in OpenAI SDK 2.5.0

using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Files;
using OpenAI.VectorStores;

namespace Library.Util;

/// <summary>
/// Wraps all OpenAI SDK calls: chat completions, realtime sessions, and vector store uploads.
/// Registered as a singleton in DI.
/// </summary>
public class LLMServiceUtil
{
    private readonly OpenAIClient _client;
    private readonly string _apiKey;
    private readonly IHttpClientFactory _httpClientFactory;

    private const string ChatModel = "gpt-4.1";
    private const string RealtimeSessionUrl = "https://api.openai.com/v1/realtime/sessions";
    private const string RealtimeModel = "gpt-4o-realtime-preview-2024-12-17";

    /// <summary>
    /// Initializes <see cref="LLMServiceUtil"/> with the OpenAI API key from configuration.
    /// </summary>
    /// <param name="configuration">Application configuration (reads OpenAI:ApiKey).</param>
    /// <param name="httpClientFactory">Factory for creating <see cref="HttpClient"/> instances.</param>
    public LLMServiceUtil(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
        _client = new OpenAIClient(_apiKey);
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Sends a chat completion request to GPT-4.1 and returns the raw text response.
    /// </summary>
    /// <param name="systemPrompt">The system-level instruction for the model.</param>
    /// <param name="userContent">The user message (serialized document JSON).</param>
    /// <returns>Raw string response from the model.</returns>
    public async Task<string> CompleteAsync(string systemPrompt, string userContent)
    {
        var chatClient = _client.GetChatClient(ChatModel);

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userContent)
        };

        var completion = await chatClient.CompleteChatAsync(messages);
        var text = completion.Value.Content.FirstOrDefault()?.Text;
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("The model returned an empty response.");
        return text;
    }

    /// <summary>
    /// Creates an OpenAI Realtime session for the given user and returns the client secret.
    /// </summary>
    /// <param name="userName">Display name of the user starting the session.</param>
    /// <returns>The <c>client_secret.value</c> string from the Realtime API response.</returns>
    public async Task<string> CreateChatSessionAsync(string userName)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new
        {
            model = RealtimeModel,
            modalities = new[] { "text" },
            // userName is JSON-serialized and transport-safe; prompt injection risk is accepted for this demo scope
            instructions = $"You are a helpful assistant. The user's name is {userName}."
        };

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(RealtimeSessionUrl, content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        dynamic? result = JsonConvert.DeserializeObject(responseBody);

        return result?.client_secret?.value?.ToString()
            ?? throw new InvalidOperationException("Could not retrieve client_secret from Realtime API response.");
    }

    /// <summary>
    /// Uploads a file to the specified OpenAI vector store and retains only the most recent file.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the target vector store.</param>
    /// <param name="fileStream">Stream of the file to upload.</param>
    /// <param name="fileName">Name of the file (used for identification in the vector store).</param>
    public async Task UploadToVectorStoreAsync(string vectorStoreId, Stream fileStream, string fileName)
    {
        var fileClient = _client.GetOpenAIFileClient();
        var uploadedFile = await fileClient.UploadFileAsync(fileStream, fileName, FileUploadPurpose.Assistants);

        var vectorStoreClient = _client.GetVectorStoreClient();
        await vectorStoreClient.AddFileToVectorStoreAsync(vectorStoreId, uploadedFile.Value.Id);

        // Retain only the most recently uploaded file — delete all older ones
        await foreach (var vsFile in vectorStoreClient.GetVectorStoreFilesAsync(vectorStoreId))
        {
            if (vsFile.FileId != uploadedFile.Value.Id)
            {
                try
                {
                    await vectorStoreClient.RemoveFileFromVectorStoreAsync(vectorStoreId, vsFile.FileId);
                    await fileClient.DeleteFileAsync(vsFile.FileId);
                }
                catch
                {
                    // Cleanup is best-effort; the newly uploaded file is already live in the vector store
                }
            }
        }
    }
}
