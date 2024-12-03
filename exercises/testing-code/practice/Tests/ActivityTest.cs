using Temporalio.Testing;
using TemporalioDurableExecution;
using Xunit;

namespace Test;

public class TranslationActivityTests
{
    private readonly HttpClient httpClient = new HttpClient();

    [Fact]
    public async Task TestSuccessfulTranslateActivityHelloGermanAsync()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Hello", "de");

        var activities = new DurableExecutionActivities(httpClient);
        var result = await env.RunAsync(() => activities.TranslateTermAsync(input));

        Assert.Equal("Hallo".ToLower(), result.Translation.ToLower());
    }

    [Fact]
    public async Task TestSuccessfulTranslateActivityGoodbyeLatvianAsync()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Goodbye", "lv");

        var activities = new DurableExecutionActivities(httpClient);
        var result = await env.RunAsync(() => activities.TranslateTermAsync(input));

        Assert.Equal("Ardievu".ToLower(), result.Translation.ToLower());
    }

    [Fact]
    public async Task TestFailedTranslateActivityBadLanguageCodeAsync()
    {
        var env = new ActivityEnvironment();
        var input = new TranslationActivityInput("Hello", "xq");

        var activities = new DurableExecutionActivities(httpClient);

        Task<TranslationActivityOutput> ActAsync() => env.RunAsync(() => activities.TranslateTermAsync(input));

        var exception = await Assert.ThrowsAsync<HttpRequestException>(ActAsync);
        Assert.Equal("HTTP Error BadRequest: \"Unsupported language code: xq\"", exception.Message);
    }
}
