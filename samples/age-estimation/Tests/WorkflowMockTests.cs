using Xunit;
using Temporalio.Testing;
using Temporalio.Client;
using Temporalio.Worker;
using Temporalio.Activities;
using AgeEstimation;

namespace AgeEstimation.Tests;

public class WorkflowMockTests
{
    [Fact]
    public async Task TestWithMockActivity()
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        [Activity]
        static Task<int> MockRetrieveEstimate(string name)
        {
            return Task.FromResult(name == "Stanislav" ? 68 : 0);
        }

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions("test-task-queue")
                .AddActivity(MockRetrieveEstimate)
                .AddWorkflow<AgeEstimationWorkflow>());

        await worker.ExecuteAsync(async () =>
        {
            var result = await env.Client.ExecuteWorkflowAsync(
                (AgeEstimationWorkflow wf) => wf.RunAsync("Stanislav"),
                new(id: $"workflow-{Guid.NewGuid()}", taskQueue: worker.Options.TaskQueue!));
            Assert.Equal("Stanislav has an estimated age of 68", result);
        });
    }
}