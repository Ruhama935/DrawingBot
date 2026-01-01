using server.DTOs;
using server.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class OpenRouterAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenRouterAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _configuration["OpenRouter:ApiKey"]
            );

        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "DrawingBot");
    }

    public async Task<string> AskAsync(string userPrompt)
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
            throw new ArgumentException("Prompt cannot be empty");

        var systemPrompt = """
You are a JSON generator.
The canvas size is width=900 and height=460
You MUST return ONLY valid JSON.
Do NOT return explanations, text, markdown, or code blocks.
Do NOT add comments.
If you cannot comply, return an empty JSON array [].

The JSON MUST be an array of drawing commands.

Allowed schemas:

circle:
{
  "type": "circle",
  "x": number,
  "y": number,
  "radius": number,
  "color"?: string
}

line:
{
  "type": "line",
  "from": { "x": number, "y": number },
  "to": { "x": number, "y": number },
  "width"?: number,
  "color"?: string
}

rect:
{
  "type": "rect",
  "x": number,
  "y": number,
  "width": number,
  "height": number,
  "color"?: string
}
""";

        var body = new
        {
            model = "meta-llama/llama-3-8b-instruct",
            messages = new[]
            {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = userPrompt }
        },
            temperature = 0.2
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                content
            );
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Failed to connect to AI service", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("AI service request timed out", ex);
        }

        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"AI service error: {response.StatusCode} - {responseText}");

        using var doc = JsonDocument.Parse(responseText);

        if (!doc.RootElement.TryGetProperty("choices", out var choices) ||
        choices.GetArrayLength() == 0)
        {
            throw new Exception("AI service returned invalid response structure");
        }

        var contentStr =  doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!;

        if (string.IsNullOrWhiteSpace(contentStr))
            throw new Exception("AI service returned empty content");

        return contentStr;
    }

}