using TemporalAgeEstimation;
using Temporalio.Testing;
using Xunit;

namespace TemporalAgeEstimation.Tests;

public class ActivityTests
{
    [Fact]
    public async Task TestRetrieveEstimateAsync()
    {
        using var httpClient = new HttpClient();
        var environment = new ActivityEnvironment();
        var activities = new AgeEstimationActivities(httpClient);

        var result = await environment.RunAsync(
            () => activities.RetrieveEstimateAsync("Betty"));

        Assert.Equal(78, result);
    }
}