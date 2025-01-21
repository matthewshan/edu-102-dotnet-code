using Microsoft.Extensions.Logging;
using TemporalAgeEstimation.Workflow;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using Xunit;

namespace TemporalAgeEstimation.Tests;

public class WorkflowTests
{
    [Fact]
    public async Task TestSuccessfulAgeEstimationAsync()
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        using var httpClient = new HttpClient();
        var activities = new AgeEstimationActivities(httpClient);

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions("test-task-queue")
                .AddWorkflow<AgeEstimationWorkflow>()
                .AddAllActivities(activities));

        await worker.ExecuteAsync(async () =>
        {
            var result = await env.Client.ExecuteWorkflowAsync(
                (AgeEstimationWorkflow wf) => wf.RunAsync("Betty"),
                new WorkflowOptions
                {
                    Id = $"workflow-{Guid.NewGuid()}",
                    TaskQueue = "test-task-queue",
                });

            Assert.Equal("Betty has an estimated age of 78", result);
        });
    }
}