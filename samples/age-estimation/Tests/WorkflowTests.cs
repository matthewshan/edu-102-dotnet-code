using Xunit;
using Temporalio.Testing;
using Temporalio.Client;
using Temporalio.Worker;
using AgeEstimation;

namespace AgeEstimation.Tests;

public class WorkflowTests
{
    [Fact]
    public async Task TestSuccessfulAgeEstimation()
    {
        await using var env = await WorkflowEnvironment.StartLocalAsync();

        var activities = new AgeEstimationActivities();

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
                    TaskQueue = worker.Options.TaskQueue!
                });

            Assert.Equal("Betty has an estimated age of 77", result);
        });
    }
}