using Library.Util;
using Microsoft.Extensions.Configuration;

namespace Library.BLL;

/// <summary>
/// Handles uploading files to the OpenAI vector store to extend the knowledge base.
/// </summary>
public class KnowledgeBaseBLL
{
    private readonly LLMServiceUtil _llmService;
    private readonly string _vectorStoreId;

    /// <summary>
    /// Initializes <see cref="KnowledgeBaseBLL"/> with the LLM service and configuration.
    /// </summary>
    /// <param name="llmService">The utility used to call OpenAI.</param>
    /// <param name="configuration">Application configuration (reads OpenAI:VectorStoreId).</param>
    public KnowledgeBaseBLL(LLMServiceUtil llmService, IConfiguration configuration)
    {
        _llmService = llmService;
        _vectorStoreId = configuration["OpenAI:VectorStoreId"]
            ?? throw new InvalidOperationException("OpenAI:VectorStoreId is not configured.");
    }

    /// <summary>
    /// Uploads a file to the vector store. Only the most recent file is retained after upload.
    /// </summary>
    /// <param name="fileStream">Stream of the file content to upload.</param>
    /// <param name="fileName">File name used for identification in the vector store.</param>
    public async Task UpdateAsync(Stream fileStream, string fileName)
    {
        await _llmService.UploadToVectorStoreAsync(_vectorStoreId, fileStream, fileName);
    }
}
