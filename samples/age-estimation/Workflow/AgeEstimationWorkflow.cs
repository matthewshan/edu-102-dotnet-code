using Temporalio.Workflows;

namespace AgeEstimation;

[Workflow]
public class AgeEstimationWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(string name)
    {
        var age = await Workflow.ExecuteActivityAsync(
            (AgeEstimationActivities act) => act.RetrieveEstimate(name),
            new() { StartToCloseTimeout = TimeSpan.FromSeconds(5) });

        return $"{name} has an estimated age of {age}";
    }
}