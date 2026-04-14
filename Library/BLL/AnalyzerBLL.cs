using System.Net;
using Library.DTO;
using Library.Util;
using Newtonsoft.Json;

namespace Library.BLL;

/// <summary>
/// Orchestrates document analysis: builds the prompt, calls GPT-4.1, and deserializes the result.
/// </summary>
public class AnalyzerBLL
{
    private readonly LLMServiceUtil _llmService;

    /// <summary>
    /// Initializes <see cref="AnalyzerBLL"/> with the LLM service utility.
    /// </summary>
    /// <param name="llmService">The utility used to call OpenAI.</param>
    public AnalyzerBLL(LLMServiceUtil llmService)
    {
        _llmService = llmService;
    }

    /// <summary>
    /// Analyzes the document described in <paramref name="request"/> and returns a structured result.
    /// </summary>
    /// <param name="request">The analysis request containing document type and content.</param>
    /// <returns>Structured <see cref="AnalysisResultDTO"/> produced by GPT-4.1.</returns>
    /// <exception cref="AppExceptionUtil">
    /// Thrown with HTTP 500 when the model returns malformed JSON.
    /// </exception>
    public async Task<AnalysisResultDTO> AnalyzeAsync(RequestDTO request)
    {
        var systemPrompt = PromptsBLL.GetPrompt(request.DocumentType);
        var serializedContent = JsonConvert.SerializeObject(request.DocumentContent);

        var rawResponse = await _llmService.CompleteAsync(systemPrompt, serializedContent);

        // Strip markdown code fences that the model occasionally adds despite explicit instructions
        var cleaned = rawResponse.Trim();
        if (cleaned.StartsWith("```"))
        {
            var firstNewline = cleaned.IndexOf('\n');
            if (firstNewline >= 0) cleaned = cleaned[(firstNewline + 1)..];
            var lastFence = cleaned.LastIndexOf("```");
            if (lastFence >= 0) cleaned = cleaned[..lastFence].Trim();
        }

        try
        {
            var result = JsonConvert.DeserializeObject<AnalysisResultDTO>(cleaned);

            if (result is null)
                throw new AppExceptionUtil(
                    "The model returned a null or empty response.",
                    HttpStatusCode.InternalServerError);

            return result;
        }
        catch (JsonException ex)
        {
            throw new AppExceptionUtil(
                $"Failed to parse the model response as a valid analysis result. " +
                $"Parser error: {ex.Message}. Raw response (first 500 chars): {rawResponse[..Math.Min(500, rawResponse.Length)]}",
                HttpStatusCode.InternalServerError);
        }
    }
}
