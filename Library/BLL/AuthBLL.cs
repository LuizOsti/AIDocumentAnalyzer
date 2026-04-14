using System.Net;
using Library.Util;
using Microsoft.Extensions.Configuration;

namespace Library.BLL;

/// <summary>
/// Validates API tokens against the list of valid GUIDs configured in appsettings.
/// </summary>
public class AuthBLL
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes <see cref="AuthBLL"/> with application configuration.
    /// </summary>
    /// <param name="configuration">Application configuration (reads Auth:ValidTokens).</param>
    public AuthBLL(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Validates a token against the configured list of valid GUIDs.
    /// Throws <see cref="AppExceptionUtil"/> with HTTP 401 if the token is invalid or missing.
    /// </summary>
    /// <param name="token">The token string from the request header.</param>
    public void Validate(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new AppExceptionUtil("Invalid or missing token.", HttpStatusCode.Unauthorized);

        var validTokens = _configuration.GetSection("Auth:ValidTokens").Get<string[]>() ?? [];

        if (!validTokens.Contains(token, StringComparer.OrdinalIgnoreCase))
            throw new AppExceptionUtil("Invalid or missing token.", HttpStatusCode.Unauthorized);
    }
}
