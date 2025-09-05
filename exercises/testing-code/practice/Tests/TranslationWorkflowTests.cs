namespace TemporalDurableExecution.Tests;

using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using TemporalioDurableExecution.Workflow;
using Xunit;

public class TranslationWorkflowTests
{
    private static readonly HttpClient Client = new HttpClient();

    [Fact]
    public async Task TestSuccessfulCompleteFrenchTranslationAsync()
    {
        var taskQueueId = Guid.NewGuid().ToString();
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        var activities = new Activities(Client);

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueueId)
                .AddWorkflow<TranslationWorkflow>()
                .AddActivity(activities.TranslateTermAsync));

        await worker.ExecuteAsync(async () =>
        {
            var input = new TranslationWorkflow.TranslationWorkflowInput("Pierre", "fr");
            var result = await env.Client.ExecuteWorkflowAsync(
                (TranslationWorkflow wf) => wf.RunAsync(input),
                new(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId));

            // TODO: Assert that the HelloMessage field in the
            //       result is: bonjour, Pierre
            Assert.Equal("bonjour, Pierre", result.HelloMessage);
            // TODO: Assert that the GoodbyeMessage field in the
            //       result is: au revoir, Pierre
            Assert.Equal("au revoir, Pierre", result.GoodbyeMessage);
        });
    }
}