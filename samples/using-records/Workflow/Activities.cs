using System.Text.Json;
using Temporalio.Activities;

namespace TemporalioDurableExecution;

public record TranslationActivityInput(string Term, string LanguageCode);
public record TranslationActivityOutput(string Translation);

public class DurableExecutionActivities
{
    private readonly HttpClient client;

    public DurableExecutionActivities(HttpClient client) => this.client = client;

    [Activity]
    public async Task<TranslationActivityOutput> TranslateTermAsync(TranslationActivityInput input)
    {
        var lang = Uri.EscapeDataString(input.LanguageCode);
        var term = Uri.EscapeDataString(input.Term.ToLower());
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"HTTP Error {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);
        var translation = jsonResponse.GetProperty("translation").GetString() ?? string.Empty;
        return new TranslationActivityOutput(translation);
    }
}