using Library.BLL;
using Library.DTO;
using Library.Util;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AIDocumentAnalyzer.Controllers;

/// <summary>
/// Endpoint for creating interactive chat sessions via the OpenAI Realtime API.
/// </summary>
[Route("api/[controller]")]
public class ChatController : BaseController
{
    private readonly ChatBLL _chatBll;
    private readonly AuthBLL _authBll;

    /// <summary>
    /// Initializes <see cref="ChatController"/> with required BLL dependencies.
    /// </summary>
    public ChatController(ChatBLL chatBll, AuthBLL authBll)
    {
        _chatBll = chatBll;
        _authBll = authBll;
    }

    /// <summary>
    /// Creates a new Realtime chat session and returns the client secret.
    /// </summary>
    /// <param name="token">Authentication token (must be a valid GUID from configuration).</param>
    /// <param name="user">Display name of the user starting the session.</param>
    /// <returns>A <see cref="ResponseDTO"/> wrapping the client secret string.</returns>
    /// <response code="200">Session created successfully.</response>
    /// <response code="401">Token is invalid or missing.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [HttpGet("start")]
    [ProducesResponseType(typeof(ResponseDTO), 200)]
    [ProducesResponseType(typeof(ResponseDTO), 401)]
    [ProducesResponseType(typeof(ResponseDTO), 500)]
    public async Task<IActionResult> Start(
        [FromHeader(Name = "Token")] string? token,
        [FromQuery] string user)
    {
        var response = new ResponseDTO();
        response.StartSw();

        try
        {
            _authBll.Validate(token);

            if (string.IsNullOrWhiteSpace(user))
                throw new AppExceptionUtil("The 'user' query parameter is required.", HttpStatusCode.BadRequest, nameof(user));

            var clientSecret = await _chatBll.StartSessionAsync(user);
            response.Finalize(clientSecret);
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
