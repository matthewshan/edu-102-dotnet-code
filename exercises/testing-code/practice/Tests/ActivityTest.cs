using Temporalio.Testing;
using TemporalioDurableExecution;
using Xunit;

namespace Test;

public class TranslationActivityTests
{
    private static readonly HttpClient Client = new HttpClient();

    [Fact]
    public async Task TestSuccessfulTranslateActivityHelloGermanAsync()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Hello", "de");

        var activities = new DurableExecutionActivities(Client);
        var result = await env.RunAsync(() => activities.TranslateTermAsync(input));

        Assert.Equal("hallo", result.Translation);
    }
}