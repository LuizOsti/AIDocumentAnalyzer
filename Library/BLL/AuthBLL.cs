using System.Net;
using Library.Util;
using Microsoft.Extensions.Configuration;

namespace Library.BLL;

/// <summary>
/// Validates API tokens against the list of valid GUIDs configured in appsettings.
/// </summary>
public class AuthBLL
{
    private readonly IReadOnlyList<string> _validTokens;

    /// <summary>
    /// Initializes <see cref="AuthBLL"/> with application configuration;
    /// reads Auth:ValidTokens once at startup.
    /// </summary>
    /// <param name="configuration">Application configuration (reads Auth:ValidTokens once at startup).</param>
    public AuthBLL(IConfiguration configuration)
    {
        _validTokens = configuration.GetSection("Auth:ValidTokens").Get<string[]>() ?? [];
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

        // Note: OrdinalIgnoreCase comparison is not constant-time. Acceptable for a portfolio
        // demo API — production use should replace with CryptographicOperations.FixedTimeEquals.
        if (!_validTokens.Contains(token, StringComparer.OrdinalIgnoreCase))
            throw new AppExceptionUtil("Invalid or missing token.", HttpStatusCode.Unauthorized);
    }
}
