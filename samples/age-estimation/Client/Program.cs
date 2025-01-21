using TemporalAgeEstimation;
using Temporalio.Client;

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new("localhost:7233"));

var name = args[0];
var options = new WorkflowOptions(
            id: $"age-estimation-{Guid.NewGuid()}",
            taskQueue: WorkflowConstants.TaskQueueName);

// Run workflow
var result = await client.ExecuteWorkflowAsync(
    (AgeEstimationWorkflow wf) => wf.RunAsync(name),
    options);

Console.WriteLine(result);