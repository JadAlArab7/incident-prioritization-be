namespace Incident.Services;

public interface ILlmService
{
    Task<LlmAnalysisResult?> AnalyzeIncidentAsync(string description, CancellationToken ct = default);
}

public class LlmAnalysisResult
{
    public string Severity { get; set; } = string.Empty;
    public string SuggestedActionsTaken { get; set; } = string.Empty;
}
