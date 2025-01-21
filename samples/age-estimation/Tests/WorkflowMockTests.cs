using Microsoft.Extensions.Logging;
using TemporalAgeEstimation;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using Temporalio.Workflows;
using Xunit;

namespace TemporalAgeEstimation.Tests;

public class WorkflowMockTests
{
    [Fact]
    public async Task TestWithMockActivityAsync()
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync(new()
        {
            LoggerFactory = LoggerFactory.Create(builder =>
                builder.
                    AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").
                    SetMinimumLevel(LogLevel.Information)),
        });

        [Activity("RetrieveEstimate")]
        static Task<int> MockRetrieveEstimateAsync(string name) =>
            Task.FromResult(name == "Stanislav" ? 68 : 0);

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions("test-task-queue")
                .AddActivity(MockRetrieveEstimateAsync)
                .AddWorkflow<AgeEstimationWorkflow>());

        await worker.ExecuteAsync(async () =>
        {
            var result = await env.Client.ExecuteWorkflowAsync(
                (AgeEstimationWorkflow wf) => wf.RunAsync("Stanislav"),
                new WorkflowOptions
                {
                    Id = $"workflow-{Guid.NewGuid()}",
                    TaskQueue = "test-task-queue",
                });

            Assert.Equal("Stanislav has an estimated age of 68", result);
        });
    }
}