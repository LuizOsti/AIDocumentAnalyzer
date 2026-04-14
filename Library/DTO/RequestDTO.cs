using Library.Util;

namespace Library.DTO;

/// <summary>
/// Request payload for the document analysis endpoint.
/// </summary>
public class RequestDTO
{
    /// <summary>Gets or sets the type of document being analyzed.</summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the document content as a dynamic JSON object.
    /// Any valid JSON structure is accepted.
    /// </summary>
    public object? DocumentContent { get; set; }
}
