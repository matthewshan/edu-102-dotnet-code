using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace TemporalioDurableExecution;

public record TranslationActivityInput(string Term, string LanguageCode);
public record TranslationActivityOutput(string Translation);

public class DurableExecutionActivities
{
    private static readonly HttpClient Client = new();

    [Activity]
    public async Task<TranslationActivityOutput> TranslateTermAsync(TranslationActivityInput input)
    {
        // TODO Part B: Define an Activity logger  
        // TODO Part B: At the Debug level, include a logging statement that lets the user know which term is being translated
        // As well as which language
        var lang = Uri.EscapeDataString(input.LanguageCode);
        var term = Uri.EscapeDataString(input.Term);
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";
        var response = await Client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"HTTP Error {response.StatusCode}: {response.Content}");
        }

        var content = await response.Content.ReadAsStringAsync();
        // TODO Part B: At the Debug level, include a logging statement that lets the user know the translation is successful
        // and include the translated content.

        return new TranslationActivityOutput(content);
    }
}