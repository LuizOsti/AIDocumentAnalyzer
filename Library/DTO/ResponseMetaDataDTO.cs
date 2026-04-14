namespace Library.DTO;

/// <summary>
/// Represents error metadata returned in failed responses.
/// </summary>
public class ResponseMetaDataDTO
{
    /// <summary>Gets or sets the field that caused the error, if applicable.</summary>
    public string? FieldName { get; set; }

    /// <summary>Gets or sets the human-readable error message.</summary>
    public string? Message { get; set; }
}
