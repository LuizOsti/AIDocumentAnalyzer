namespace Library.Util;

/// <summary>
/// Identifies the type of document being submitted for analysis.
/// Each type maps to a distinct prompt template in <c>PromptsBLL</c>.
/// </summary>
public enum DocumentType
{
    /// <summary>A legal contract or agreement.</summary>
    Contract,

    /// <summary>A financial report, balance sheet, or earnings statement.</summary>
    FinancialReport,

    /// <summary>A profile describing an individual person.</summary>
    PersonProfile,

    /// <summary>A profile describing a company or organization.</summary>
    CompanyProfile,

    /// <summary>Any document that does not fit the above categories.</summary>
    Generic
}
