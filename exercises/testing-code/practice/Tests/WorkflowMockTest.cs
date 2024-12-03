using Temporalio.Client;
using Temporalio.Worker;
using Temporalio.Testing;
using TemporalioDurableExecution;
using Xunit;

namespace Test;

public class TranslationWorkflowMockTests
{
    private readonly HttpClient httpClient = new HttpClient();

    [Fact]
    public async Task TestSuccessfulTranslationWithMocks()
    {
        var taskQueueId = Guid.NewGuid().ToString();

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        var activities = new DurableExecutionActivities(httpClient);

        using var worker = new TemporalWorker(env.Client,
            new TemporalWorkerOptions(taskQueueId)
                .AddWorkflow<TranslationWorkflow>()
                .AddActivity(activities.TranslateTermAsync));

        await worker.ExecuteAsync(async () =>
        {
            var input = new TranslationWorkflowInput("Pierre", "fr");

            var result = await env.Client.ExecuteWorkflowAsync(
                (TranslationWorkflow wf) => wf.RunAsync(input),
                new(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId));

            Assert.Equal("Bonjour, Pierre".ToLower(), result.HelloMessage.ToLower());
            Assert.Equal("Au revoir, Pierre".ToLower(), result.GoodbyeMessage.ToLower());
        });
    }
}
