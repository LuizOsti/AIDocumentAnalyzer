using System.Net;

namespace Library.Util;

/// <summary>
/// Application-level exception that carries an HTTP status code
/// and an optional field name for validation errors.
/// </summary>
public class AppExceptionUtil : Exception
{
    /// <summary>Gets the HTTP status code to return to the caller.</summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>Gets the name of the field that caused the error, if applicable.</summary>
    public string? FieldName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AppExceptionUtil"/>.
    /// </summary>
    /// <param name="message">Human-readable error message.</param>
    /// <param name="statusCode">HTTP status code to return.</param>
    /// <param name="fieldName">Optional field name that caused the error.</param>
    public AppExceptionUtil(string message, HttpStatusCode statusCode, string? fieldName = null)
        : base(message)
    {
        StatusCode = statusCode;
        FieldName = fieldName;
    }
}
