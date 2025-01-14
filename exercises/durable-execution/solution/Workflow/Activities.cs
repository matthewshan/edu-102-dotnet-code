using System.Text.Json;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace TemporalioDurableExecution.Workflow;

public class Activities
{
    private readonly HttpClient client;

    public Activities(HttpClient client) => this.client = client;

    public record TranslateTermInput(string Term, string LanguageCode);
    public record TranslateTermOutput(string Translation);

    [Activity]
    public async Task<TranslateTermOutput> TranslateTermAsync(TranslateTermInput input)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("Translating term {Term} to {LanguageCode}", input.Term, input.LanguageCode);

        var lang = Uri.EscapeDataString(input.LanguageCode);
        var term = Uri.EscapeDataString(input.Term.ToLower());
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        logger.LogInformation("Translation successful. Translation: {Translation}", content);
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);
        var translation = jsonResponse.GetProperty("translation").GetString() ?? string.Empty;
        return new TranslateTermOutput(translation);
    }
}