namespace Library.DTO;

/// <summary>Root result returned by the analysis endpoint.</summary>
public class AnalysisResultDTO
{
    /// <summary>High-level executive summary of the analysis.</summary>
    public ExecutiveSummaryDTO? ExecutiveSummary { get; set; }

    /// <summary>Detailed findings grouped by category.</summary>
    public List<FindingsByCategoryDTO> FindingsByCategory { get; set; } = [];

    /// <summary>Fraud risk indicators identified in the document.</summary>
    public FraudIndicatorsDTO? FraudIndicators { get; set; }

    /// <summary>Risk classification broken down by topic.</summary>
    public List<RiskClassificationDTO> RiskClassificationByTopic { get; set; } = [];

    /// <summary>Prioritized action plans recommended after analysis.</summary>
    public List<ActionPlanDTO> RecommendedActionPlans { get; set; } = [];

    /// <summary>Improvement suggestions for the analyzed document.</summary>
    public ImprovementSuggestionsDTO? ImprovementSuggestions { get; set; }
}

/// <summary>High-level executive summary.</summary>
public class ExecutiveSummaryDTO
{
    /// <summary>Overall risk level: Low, Medium, High, or Critical.</summary>
    public string OverallRisk { get; set; } = string.Empty;

    /// <summary>List of positive points identified.</summary>
    public List<string> PositivePoints { get; set; } = [];

    /// <summary>List of points requiring attention.</summary>
    public List<string> AttentionPoints { get; set; } = [];

    /// <summary>List of critical alert points.</summary>
    public List<string> AlertPoints { get; set; } = [];
}

/// <summary>Findings grouped under a single category.</summary>
public class FindingsByCategoryDTO
{
    /// <summary>Category name (e.g., Legal, Financial, Compliance).</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>List of observations within this category.</summary>
    public List<string> Observations { get; set; } = [];
}

/// <summary>Fraud indicators detected in the document.</summary>
public class FraudIndicatorsDTO
{
    /// <summary>Disclaimer clarifying the statistical nature of the indicators.</summary>
    public string Disclaimer { get; set; } = string.Empty;

    /// <summary>Individual fraud indicator points.</summary>
    public List<string> Points { get; set; } = [];
}

/// <summary>Risk level for a specific topic area.</summary>
public class RiskClassificationDTO
{
    /// <summary>Topic name (e.g., Financial, Legal).</summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>Risk level: Low, Medium, High, or Critical.</summary>
    public string Risk { get; set; } = string.Empty;
}

/// <summary>A prioritized set of recommended actions.</summary>
public class ActionPlanDTO
{
    /// <summary>Priority level: High, Medium, Low, or Monitor.</summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>Actions to take at this priority level.</summary>
    public List<string> Actions { get; set; } = [];
}

/// <summary>Improvement suggestions for the analyzed document.</summary>
public class ImprovementSuggestionsDTO
{
    /// <summary>General comment on improvement areas.</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>Specific improvement suggestions.</summary>
    public List<string> Suggestions { get; set; } = [];
}
