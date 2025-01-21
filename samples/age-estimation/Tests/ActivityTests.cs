namespace TemporalAgeEstimation.Tests;

using Xunit;
using Temporalio.Testing;
using TemporalAgeEstimation.Workflow;

public class ActivityTests
{
    [Fact]
    public async Task TestRetrieveEstimateAsync()
    {
        // we're intentionally making an HTTP call in a test here
        using var httpClient = new HttpClient();
        var environment = new ActivityEnvironment();
        var activities = new AgeEstimationActivities(httpClient);

        var result = await environment.RunAsync(
            () => activities.RetrieveEstimateAsync("Betty"));

        Assert.Equal(78, result);
    }
}