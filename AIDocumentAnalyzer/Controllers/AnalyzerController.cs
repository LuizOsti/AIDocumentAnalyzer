using Library.BLL;
using Library.DTO;
using Library.Util;
using Microsoft.AspNetCore.Mvc;

namespace AIDocumentAnalyzer.Controllers;

/// <summary>
/// Endpoint for analyzing documents with GPT-4.1.
/// </summary>
[Route("api/[controller]")]
public class AnalyzerController : BaseController
{
    private readonly AnalyzerBLL _analyzerBll;
    private readonly AuthBLL _authBll;

    /// <summary>
    /// Initializes <see cref="AnalyzerController"/> with required BLL dependencies.
    /// </summary>
    public AnalyzerController(AnalyzerBLL analyzerBll, AuthBLL authBll)
    {
        _analyzerBll = analyzerBll;
        _authBll = authBll;
    }

    /// <summary>
    /// Analyzes a document and returns a structured risk analysis.
    /// </summary>
    /// <param name="token">Authentication token (must be a valid GUID from configuration).</param>
    /// <param name="request">The document type and content to analyze.</param>
    /// <returns>A <see cref="ResponseDTO"/> wrapping an <see cref="AnalysisResultDTO"/>.</returns>
    /// <response code="200">Analysis completed successfully.</response>
    /// <response code="401">Token is invalid or missing.</response>
    /// <response code="500">An unexpected error occurred during analysis.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDTO), 200)]
    [ProducesResponseType(typeof(ResponseDTO), 401)]
    [ProducesResponseType(typeof(ResponseDTO), 500)]
    public async Task<IActionResult> Post(
        [FromHeader(Name = "Token")] string? token,
        [FromBody] RequestDTO request)
    {
        var response = new ResponseDTO();
        response.StartSw();

        try
        {
            _authBll.Validate(token);
            var result = await _analyzerBll.AnalyzeAsync(request);
            response.Finalize(result);
        }
        catch (AppExceptionUtil ex)
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
