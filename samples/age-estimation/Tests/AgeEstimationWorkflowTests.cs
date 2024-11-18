using Microsoft.Extensions.DependencyInjection;
using AgeEstimation.Activities;
using AgeEstimation.Workflows;
using Temporalio.Testing;
using Xunit;

namespace AgeEstimation.Tests;

public class AgeEstimationWorkflowTests
{
    [Fact]
    public async Task TestSuccessfulAgeEstimation()
    {
        using var host = await TestWorkflowHost.StartAsync();
        
        // Create a test-specific Activity implementation
        var mockActivities = new TestAgeEstimationActivities();

        var worker = await host.StartWorkerAsync(
            options => options
                .AddWorkflow<AgeEstimationWorkflow>()
                .AddActivity(mockActivities));

        var client = host.Client;
        var result = await client.ExecuteWorkflowAsync<string>(
            (AgeEstimationWorkflow wf) => wf.RunAsync("Betty"),
            new WorkflowOptions { TaskQueue = "test" });

        Assert.Equal("Betty has an estimated age of 76", result);
    }
}

public class TestAgeEstimationActivities : AgeEstimationActivities
{
    public override Task<int> RetrieveEstimate(string name)
    {
        return Task.FromResult(76);
    }
}