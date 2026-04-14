using System.Diagnostics;
using Library.Util;

namespace Library.DTO;

/// <summary>
/// Standard response envelope for all API endpoints.
/// </summary>
public class ResponseDTO
{
    private readonly Stopwatch _sw = new();

    /// <summary>Gets or sets the response payload.</summary>
    public object? Return { get; set; }

    /// <summary>Gets or sets whether the operation succeeded.</summary>
    public bool Success { get; set; }

    /// <summary>Gets or sets the status label ("Ok" or "Error").</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the total elapsed time in milliseconds.</summary>
    public long ElapsedTimeMs { get; set; }

    /// <summary>Gets or sets the UTC timestamp when the request was processed.</summary>
    public DateTime DateTimeExecution { get; set; }

    /// <summary>Gets or sets the HTTP status code of the response.</summary>
    public int HttpStatusCode { get; set; }

    /// <summary>Gets or sets the error metadata, populated only on failure.</summary>
    public ResponseMetaDataDTO? Error { get; set; }

    /// <summary>Starts the internal stopwatch. Call this at the beginning of every request.</summary>
    public void StartSw() => _sw.Start();

    /// <summary>Finalizes a successful response, stopping the stopwatch and setting all success fields.</summary>
    /// <param name="result">The payload to return to the caller.</param>
    public void Finalize(object result)
    {
        _sw.Stop();
        Return = result;
        Success = true;
        Status = "Ok";
        HttpStatusCode = 200;
        ElapsedTimeMs = _sw.ElapsedMilliseconds;
        DateTimeExecution = DateTime.UtcNow;
    }

    /// <summary>Finalizes an error response from a known application exception.</summary>
    /// <param name="ex">The application exception containing status code and field name.</param>
    public void FinalizeError(AppExceptionUtil ex)
    {
        _sw.Stop();
        Return = null;
        Success = false;
        Status = "Error";
        HttpStatusCode = (int)ex.StatusCode;
        ElapsedTimeMs = _sw.ElapsedMilliseconds;
        DateTimeExecution = DateTime.UtcNow;
        Error = new ResponseMetaDataDTO
        {
            FieldName = ex.FieldName,
            Message = ex.Message
        };
    }

    /// <summary>Finalizes an error response from an unexpected exception (HTTP 500).</summary>
    /// <param name="ex">The unhandled exception.</param>
    public void FinalizeError(Exception ex)
    {
        _sw.Stop();
        Return = null;
        Success = false;
        Status = "Error";
        HttpStatusCode = 500;
        ElapsedTimeMs = _sw.ElapsedMilliseconds;
        DateTimeExecution = DateTime.UtcNow;
        Error = new ResponseMetaDataDTO
        {
            Message = ex.Message
        };
    }
}
