using Library.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AIDocumentAnalyzer.Controllers;

/// <summary>
/// Base controller providing shared response handling for all endpoints.
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Maps a <see cref="ResponseDTO"/> to the appropriate HTTP result
    /// based on its <see cref="ResponseDTO.HttpStatusCode"/>.
    /// </summary>
    /// <param name="response">The populated response envelope.</param>
    /// <returns>An <see cref="IActionResult"/> with the correct HTTP status code.</returns>
    protected IActionResult ValidateReturn(ResponseDTO response) =>
        response.HttpStatusCode switch
        {
            200 => Ok(response),
            400 => BadRequest(response),
            401 => Unauthorized(response),
            _ => StatusCode(500, response)
        };
}
