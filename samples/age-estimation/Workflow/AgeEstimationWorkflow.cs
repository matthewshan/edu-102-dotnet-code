using Temporalio.Workflows;

namespace TemporalAgeEstimation;

[Workflow]
public class AgeEstimationWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(string name)
    {
        var age = await Temporalio.Workflows.Workflow.ExecuteActivityAsync(
            (AgeEstimationActivities act) => act.RetrieveEstimateAsync(name),
            new() { StartToCloseTimeout = TimeSpan.FromSeconds(30) });
        return $"{name} has an estimated age of {age}";
    }
}