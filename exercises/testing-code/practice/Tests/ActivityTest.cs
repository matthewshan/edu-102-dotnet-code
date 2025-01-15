namespace Test;

using Temporalio.Testing;
using TemporalioDurableExecution;
using Xunit;

public class TranslationActivityTests
{
    private static readonly HttpClient Client = new HttpClient();

    [Fact]
    public async Task TestSuccessfulTranslateActivityHelloGermanAsync()
    {
        var env = new ActivityEnvironment();
        var input = new Activities.TranslateTermInput("Hello", "de");

        var activities = new DurableExecutionActivities(Client);
        var result = await env.RunAsync(() => Activities.TranslateTermAsync(input));

        Assert.Equal("hallo", result.Translation);
    }
}