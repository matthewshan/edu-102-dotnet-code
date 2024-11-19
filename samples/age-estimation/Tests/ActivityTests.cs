using Xunit;
using Temporalio.Testing;
using AgeEstimation;

namespace AgeEstimation.Tests;

public class ActivityTests
{
    [Fact]
    public async Task TestRetrieveEstimate()
    {
        var environment = new ActivityEnvironment();
        var activities = new AgeEstimationActivities();

        var result = await environment.RunAsync(
            () => activities.RetrieveEstimateAsync("Angela"));

        Assert.Equal(56, result);
    }
}