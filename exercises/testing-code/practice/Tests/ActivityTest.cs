namespace TemporalDurableExecution.Tests;

using Temporalio.Testing;
using TemporalioDurableExecution.Workflow;
using Xunit;

public class TranslationActivityTests
{
    private static readonly HttpClient Client = new HttpClient();

    [Fact]
    public async Task TestSuccessfulTranslateActivityHelloGermanAsync()
    {
        var env = new ActivityEnvironment();
        var input = new Activities.TranslateTermInput("Hello", "de");

        var activities = new Activities(Client);
        var result = await env.RunAsync(() => activities.TranslateTermAsync(input));

        Assert.Equal("hallo", result.Translation);
    }
}