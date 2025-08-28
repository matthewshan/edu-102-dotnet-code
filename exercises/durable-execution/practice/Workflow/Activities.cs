namespace TemporalioDurableExecution.Workflow;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

public class Activities
{
    private readonly HttpClient client;

    public Activities(HttpClient client) => this.client = client;

    public record TranslateTermInput(string Term, string LanguageCode);

    public record TranslateTermOutput(string Translation);

    [Activity]
    public async Task<TranslateTermOutput> TranslateTermAsync(TranslateTermInput input)
    {
        // TODO Part B: Define an Activity logger
        var logger = ActivityExecutionContext.Current.Logger;

        // TODO Part B: At the Information level, include a logging statement that lets the user know which term is being translated
        // As well as which language
        logger.LogInformation("Term '{Term}' with language '{Language}'", input.Term, input.LanguageCode);
        var lang = Uri.EscapeDataString(input.LanguageCode);
        var term = Uri.EscapeDataString(input.Term.ToLower());
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        // TODO Part B: At the Information level, include a logging statement that lets the user know the translation is successful
        // and include the translated content.
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);
        var translation = jsonResponse.GetProperty("translation").GetString() ?? string.Empty;
        logger.LogInformation("Translation Successful: '{Translation}'", translation);

        return new TranslateTermOutput(translation);
    }
}