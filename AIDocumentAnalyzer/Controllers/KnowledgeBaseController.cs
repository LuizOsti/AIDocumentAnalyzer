using Library.BLL;
using Library.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AIDocumentAnalyzer.Controllers;

/// <summary>
/// Endpoint for uploading files to the OpenAI vector store.
/// </summary>
[Route("api/[controller]")]
public class KnowledgeBaseController : BaseController
{
    private readonly KnowledgeBaseBLL _knowledgeBaseBll;
    private readonly AuthBLL _authBll;

    /// <summary>
    /// Initializes <see cref="KnowledgeBaseController"/> with required BLL dependencies.
    /// </summary>
    public KnowledgeBaseController(KnowledgeBaseBLL knowledgeBaseBll, AuthBLL authBll)
    {
        _knowledgeBaseBll = knowledgeBaseBll;
        _authBll = authBll;
    }

    /// <summary>
    /// Uploads a file to the OpenAI vector store. Only the most recent file is retained.
    /// </summary>
    /// <param name="token">Authentication token (must be a valid GUID from configuration).</param>
    /// <param name="file">The file to upload.</param>
    /// <returns>A <see cref="ResponseDTO"/> indicating success or failure.</returns>
    /// <response code="200">File uploaded successfully.</response>
    /// <response code="400">No file was provided.</response>
    /// <response code="401">Token is invalid or missing.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ResponseDTO), 200)]
    [ProducesResponseType(typeof(ResponseDTO), 400)]
    [ProducesResponseType(typeof(ResponseDTO), 401)]
    [ProducesResponseType(typeof(ResponseDTO), 500)]
    public async Task<IActionResult> Update(
        [FromHeader(Name = "Token")] string? token,
        IFormFile? file)
    {
        var response = new ResponseDTO();
        response.StartSw();

        try
        {
            _authBll.Validate(token);

            if (file is null || file.Length == 0)
                throw new Library.Util.AppExceptionUtil(
                    "No file was provided.",
                    System.Net.HttpStatusCode.BadRequest,
                    nameof(file));

            using var stream = file.OpenReadStream();
            await _knowledgeBaseBll.UpdateAsync(stream, file.FileName);
            response.Finalize("File uploaded successfully.");
        }
        catch (Library.Util.AppExceptionUtil ex)
        {
            response.FinalizeError(ex);
        }
        catch (Exception ex)
        {
            response.FinalizeError(ex);
        }

        return ValidateReturn(response);
    }
}
