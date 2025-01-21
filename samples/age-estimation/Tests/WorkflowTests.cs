namespace TemporalAgeEstimation.Tests;

using Xunit;
using Microsoft.Extensions.Logging;
using Temporalio.Testing;
using Temporalio.Worker;
using Temporalio.Activities;
using TemporalAgeEstimation.Workflow;

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

    [Fact]
    async Task TestWithMockActivity()
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync(new()
        {
            LoggerFactory = LoggerFactory.Create(builder =>
                builder.
                    AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").
                    SetMinimumLevel(LogLevel.Information)),
        });

        [Activity("RetrieveEstimate")]
        static Task<int> MockRetrieveEstimate(string name) =>
            Task.FromResult(name == "Stanislav" ? 68 : 0);

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions("test-task-queue")
                .AddActivity(MockRetrieveEstimate)
                .AddWorkflow<AgeEstimationWorkflow>());

        await worker.ExecuteAsync(async () =>
        {
            var result = await env.Client.ExecuteWorkflowAsync(
                (AgeEstimationWorkflow wf) => wf.RunAsync("Stanislav"),
                new WorkflowOptions
                {
                    Id = $"workflow-{Guid.NewGuid()}",
                    TaskQueue = "test-task-queue"
                });

            Assert.Equal("Betty has an estimated age of 77", result);
        });
    }

    [Fact]
    async Task TestWithMockActivity()
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync(new()
        {
            LoggerFactory = LoggerFactory.Create(builder =>
                builder.
                    AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").
                    SetMinimumLevel(LogLevel.Information)),
        });

        [Activity("RetrieveEstimate")]
        static Task<int> MockRetrieveEstimate(string name) =>
            Task.FromResult(name == "Stanislav" ? 68 : 0);

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions("test-task-queue")
                .AddActivity(MockRetrieveEstimate)
                .AddWorkflow<AgeEstimationWorkflow>());

        await worker.ExecuteAsync(async () =>
        {
            var result = await env.Client.ExecuteWorkflowAsync(
                (AgeEstimationWorkflow wf) => wf.RunAsync("Stanislav"),
                new WorkflowOptions
                {
                    Id = $"workflow-{Guid.NewGuid()}",
                    TaskQueue = "test-task-queue"
                });

            Assert.Equal("Stanislav has an estimated age of 68", result);
        });
    }
}