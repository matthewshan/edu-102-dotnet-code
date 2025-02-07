// This file is designated to run the Worker
using Microsoft.Extensions.Logging;
using Temporalio.Client;
using Temporalio.Worker;
using TemporalioDurableExecution.Workflow;

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new("localhost:7233")
{
    LoggerFactory = LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").SetMinimumLevel(LogLevel.Information)),
});

// Cancellation token to shutdown worker on ctrl+c
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

using var httpClient = new HttpClient();
var activities = new Activities(httpClient);

// Create worker
using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions(WorkflowConstants.TaskQueueName)
        .AddAllActivities(activities)
        .AddWorkflow<TranslationWorkflow>());

// Run worker until cancelled
Console.WriteLine($"Running worker...{client.Connection.Options.Identity}");

try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Worker cancelled");
}