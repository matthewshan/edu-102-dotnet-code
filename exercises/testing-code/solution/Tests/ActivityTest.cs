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

    [Fact]
    public async Task TestSuccessfulTranslateActivityGoodbyeLatvianAsync()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Goodbye", "lv");

        var activities = new DurableExecutionActivities(Client);
        var result = await env.RunAsync(() => activities.TranslateTermAsync(input));

        Assert.Equal("ardievu", result.Translation);
    }

    [Fact]
    public async Task TestFailedTranslateActivityBadLanguageCodeAsync()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Hello", "xq");

        var activities = new DurableExecutionActivities(Client);

        Task<TranslationActivityOutput> ActAsync() => env.RunAsync(() => activities.TranslateTermAsync(input));

        var exception = await Assert.ThrowsAsync<HttpRequestException>(ActAsync);
        Assert.Equal("HTTP Error BadRequest: \"Unsupported language code: xq\"", exception.Message);
    }
}