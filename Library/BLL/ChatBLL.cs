using Library.Util;

namespace Library.BLL;

/// <summary>
/// Handles creation of interactive chat sessions via the OpenAI Realtime API.
/// </summary>
public class ChatBLL
{
    private readonly LLMServiceUtil _llmService;

    /// <summary>
    /// Initializes <see cref="ChatBLL"/> with the LLM service utility.
    /// </summary>
    /// <param name="llmService">The utility used to call OpenAI.</param>
    public ChatBLL(LLMServiceUtil llmService)
    {
        _llmService = llmService;
    }

    /// <summary>
    /// Creates a new Realtime chat session for the given user and returns the client secret.
    /// </summary>
    /// <param name="userName">Display name of the user starting the session.</param>
    /// <returns>The client secret string that the frontend uses to connect to the Realtime API.</returns>
    public async Task<string> StartSessionAsync(string userName)
    {
        return await _llmService.CreateChatSessionAsync(userName);
    }
}
