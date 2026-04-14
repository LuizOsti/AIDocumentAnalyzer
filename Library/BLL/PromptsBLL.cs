using Library.Util;

namespace Library.BLL;

/// <summary>
/// Provides system prompt templates for each supported document type.
/// Prompts instruct GPT-4.1 to return only valid JSON matching <see cref="Library.DTO.AnalysisResultDTO"/>.
/// </summary>
public static class PromptsBLL
{
    private const string JsonStructureInstruction =
        """
        Base your analysis strictly on the data provided. Do not invent facts, names, or figures not present in the input.
        Return ONLY a valid JSON object with no markdown, no code fences, and no extra text.
        Use this exact structure:
        {
          "ExecutiveSummary": {
            "OverallRisk": "<Low|Medium|High|Critical>",
            "PositivePoints": ["..."],
            "AttentionPoints": ["..."],
            "AlertPoints": ["..."]
          },
          "FindingsByCategory": [
            { "Category": "...", "Observations": ["..."] }
          ],
          "FraudIndicators": {
            "Disclaimer": "These indicators are based on statistical pattern analysis and do not constitute legal proof.",
            "Points": ["..."]
          },
          "RiskClassificationByTopic": [
            { "Topic": "...", "Risk": "<Low|Medium|High|Critical>" }
          ],
          "RecommendedActionPlans": [
            { "Priority": "<High|Medium|Low|Monitor>", "Actions": ["..."] }
          ],
          "ImprovementSuggestions": {
            "Comment": "...",
            "Suggestions": ["..."]
          }
        }
        """;

    /// <summary>
    /// Returns the system prompt for the given document type.
    /// </summary>
    /// <param name="documentType">The type of document to analyze.</param>
    /// <returns>The system prompt string to pass to GPT-4.1.</returns>
    public static string GetPrompt(DocumentType documentType) => documentType switch
    {
        DocumentType.Contract => GetContractPrompt(),
        DocumentType.FinancialReport => GetFinancialReportPrompt(),
        DocumentType.PersonProfile => GetPersonProfilePrompt(),
        DocumentType.CompanyProfile => GetCompanyProfilePrompt(),
        DocumentType.Generic => GetGenericPrompt(),
        _ => GetGenericPrompt()
    };

    private static string GetContractPrompt() =>
        $"""
        You are an expert contract analyst and legal risk specialist.
        Analyze the provided contract data thoroughly, focusing on:
        - Contractual obligations and liabilities
        - Termination and penalty clauses
        - Ambiguous or missing clauses
        - Jurisdiction and governing law
        - Unusual or one-sided terms
        - Signs of fraud or document tampering
        {JsonStructureInstruction}
        """;

    private static string GetFinancialReportPrompt() =>
        $"""
        You are an expert financial analyst and forensic accountant.
        Analyze the provided financial report data thoroughly, focusing on:
        - Revenue, profitability, and cash flow trends
        - Debt levels and liquidity ratios
        - Unusual accounting entries or restatements
        - Off-balance-sheet exposure
        - Signs of earnings manipulation or fraud
        - Compliance with IFRS/GAAP principles
        {JsonStructureInstruction}
        """;

    private static string GetPersonProfilePrompt() =>
        $"""
        You are an expert in personal background analysis and KYC (Know Your Customer) compliance.
        Analyze the provided person profile data thoroughly, focusing on:
        - Identity consistency and document validity
        - Professional and financial history
        - Politically exposed person (PEP) indicators
        - Adverse media or legal history
        - Fraud risk signals (inconsistencies, forged documents)
        - AML (Anti-Money Laundering) red flags
        {JsonStructureInstruction}
        """;

    private static string GetCompanyProfilePrompt() =>
        $"""
        You are an expert in corporate due diligence and business risk analysis.
        Analyze the provided company profile data thoroughly, focusing on:
        - Corporate structure and ownership transparency
        - Financial health and credit risk
        - Regulatory compliance and licensing
        - Beneficial ownership and UBO clarity
        - Shell company or fraud indicators
        - Sanctions, watchlists, or adverse media
        {JsonStructureInstruction}
        """;

    private static string GetGenericPrompt() =>
        $"""
        You are an expert document risk analyst.
        Analyze the provided document data thoroughly, focusing on:
        - Internal consistency and completeness
        - Data authenticity and integrity
        - Anomalies, gaps, or contradictions
        - Potential fraud or manipulation signals
        - Regulatory or compliance implications
        - Overall quality and reliability of the information
        {JsonStructureInstruction}
        """;
}
