using Temporalio.Workflows;

namespace TemporalioDurableExecution;

public record TranslationWorkflowInput(string Name, string LanguageCode);
public record TranslationWorkflowOutput(string HelloMessage, string GoodbyeMessage);

[Workflow]
public class TranslationWorkflow
{
    [WorkflowRun]
    public async Task<TranslationWorkflowOutput> RunAsync(TranslationWorkflowInput input)
    {
        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        var helloResult = await Workflow.ExecuteActivityAsync(
            (DurableExecutionActivities act) => act.TranslateTermAsync(
                new TranslationActivityInput("hello", input.LanguageCode.ToLower())),
            activityOptions);

        var goodbyeResult = await Workflow.ExecuteActivityAsync(
            (DurableExecutionActivities act) => act.TranslateTermAsync(
                new TranslationActivityInput("goodbye", input.LanguageCode.ToLower())),
            activityOptions);

        return new TranslationWorkflowOutput(
            $"{helloResult.Translation}, {input.Name}",
            $"{goodbyeResult.Translation}, {input.Name}");
    }
}