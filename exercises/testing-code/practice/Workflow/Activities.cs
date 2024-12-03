using System.Text.Json;
using Microsoft.Extensions.Logging;
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
        // TODO Part B: Define an Activity logger  
        // TODO Part B: At the Information level, include a logging statement that lets the user know which term is being translated
        // As well as which language
        var lang = Uri.EscapeDataString(input.LanguageCode);
        var term = Uri.EscapeDataString(input.Term.ToLower());
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Translation failed. Status: {Status}, Message: {Message}", response.StatusCode, content);
            throw new HttpRequestException($"HTTP Error {response.StatusCode}: {content}");
        }

        // TODO Part B: At the Information level, include a logging statement that lets the user know the translation is successful
        // and include the translated content.
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);
        var translation = jsonResponse.GetProperty("translation").GetString() ?? string.Empty;
        return new TranslationActivityOutput(translation);
    }
}