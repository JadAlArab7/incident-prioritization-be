namespace Incident.Infrastructure;

public class OpenRouterSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = "https://openrouter.ai/api/v1/chat/completions";
    public string Model { get; set; } = "meta-llama/llama-3.1-8b-instruct:free";
}
