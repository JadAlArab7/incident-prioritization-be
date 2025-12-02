using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Incident.Infrastructure;
using Microsoft.Extensions.Options;

namespace Incident.Services;

public class LlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouterSettings _settings;
    private readonly ILogger<LlmService> _logger;

    public LlmService(HttpClient httpClient, IOptions<OpenRouterSettings> settings, ILogger<LlmService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<LlmAnalysisResult?> AnalyzeIncidentAsync(string description, CancellationToken ct = default)
    {
        try
        {
            var systemPrompt = @"
## Introduction
You are an expert incident analyst. Your task is to analyze the provided incident description and return:
1. **Severity level** (choose one: Low, Medium, High, Critical)
2. **Suggested actions to be taken**

## Important Rules
- Respond ONLY with a valid JSON object.
- Do NOT include any explanation or text outside the JSON object.
- If no relevant fields can be extracted, return the same JSON object with severity set to ""Low"" and suggestedActionsTaken as ""No actions identified.""

## Example
Incident Description: Armed raid occurred at downtown bank.

### Expected Output:
{
  ""severity"": ""High"",
  ""suggestedActionsTaken"": ""Notify law enforcement immediately, secure the area, evacuate civilians, and initiate emergency response protocols.""
}
Note: just analyse the incident and return the JSON object as shown above.
DO not return anything else.
I need no reasoning or explanation, just the JSON object.

Now analyze the following incident description:
";

            var requestBody = new
            {
                model = _settings.Model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Analyze this incident: {description}" }
                },
                temperature = 0.7,
                max_tokens = 500
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://incident-api.local");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "Incident Analysis API");

            var response = await _httpClient.PostAsync(_settings.ApiUrl, httpContent, ct);
            
             if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("OpenRouter API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                return null;
            }

            var responseContent = (await response.Content.ReadAsStringAsync(ct)).Trim();
            var openRouterResponse = JsonSerializer.Deserialize<OpenRouterResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (openRouterResponse?.Choices == null || openRouterResponse.Choices.Length == 0)
            {
                _logger.LogWarning("No response from OpenRouter API");
                return null;
            }

            var messageContent = openRouterResponse.Choices[0].Message?.Content;
            
            // Some models (like DeepSeek reasoning models) put content in 'reasoning' field
            if (string.IsNullOrEmpty(messageContent))
            {
                messageContent = openRouterResponse.Choices[0].Message?.Reasoning;
            }
            
            if (string.IsNullOrEmpty(messageContent))
            {
                _logger.LogWarning("Empty message content from OpenRouter API");
                return null;
            }

            // Extract JSON from the response (in case there's extra text)
            var jsonStart = messageContent.IndexOf('{');
            var jsonEnd = messageContent.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonPart = messageContent.Substring(jsonStart, jsonEnd - jsonStart + 1);
                
                try
                {
                    var result = JsonSerializer.Deserialize<LlmAnalysisResult>(jsonPart, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (result == null)
                    {
                        _logger.LogError("Deserialized result is null. JSON content: {JsonContent}", jsonPart);
                        throw new InvalidOperationException("Failed to deserialize LLM response to LlmAnalysisResult");
                    }
                    
                    // Validate that required fields are not empty
                    if (string.IsNullOrWhiteSpace(result.Severity))
                    {
                        _logger.LogError("Severity field is missing or empty in LLM response. JSON content: {JsonContent}", jsonPart);
                        throw new InvalidOperationException("LLM response is missing required 'severity' field");
                    }
                    
                    if (string.IsNullOrWhiteSpace(result.SuggestedActionsTaken))
                    {
                        _logger.LogError("SuggestedActionsTaken field is missing or empty in LLM response. JSON content: {JsonContent}", jsonPart);
                        throw new InvalidOperationException("LLM response is missing required 'suggestedActionsTaken' field");
                    }
                    
                    return result;
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize LLM response. JSON content: {JsonContent}", jsonPart);
                    throw new InvalidOperationException($"Invalid JSON format in LLM response: {jsonEx.Message}", jsonEx);
                }
            }

            _logger.LogError("Could not extract valid JSON from LLM response: {Response}", messageContent);
            throw new InvalidOperationException("LLM response did not contain valid JSON object");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenRouter API");
            return null;
        }
    }

    private class OpenRouterResponse
    {
        public Choice[]? Choices { get; set; }
    }

    private class Choice
    {
        public Message? Message { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    private class Message
    {
        public string? Content { get; set; }
        public string? Role { get; set; }
        public string? Reasoning { get; set; }
    }
}
