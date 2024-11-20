using Microsoft.Extensions.Logging;
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
        // TODO Part A: Define the Workflow logger here

        // TODO Part A: At the Info level, log that the Workflow function has been invoked.
        // Include the name passed as input.
        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        // TODO Part A: At the Information level, log a message stating that the translation will happen now
        // Include the language code passed as input
        var helloResult = await Workflow.ExecuteActivityAsync(
            (DurableExecutionActivities act) => act.TranslateTermAsync(
                new TranslationActivityInput("hello", input.LanguageCode.ToLower())),
            activityOptions);

        // TODO Part C: At the Information level, log the message: "Sleeping between translation calls"
        // TODO Part C: Use `Workflow.DelayAsync` to set a Timer for 10 seconds.

        // TODO Part A: At the Information level, log a message stating that the translation will happen now
        // Include the language code passed as input
        var goodbyeResult = await Workflow.ExecuteActivityAsync(
            (DurableExecutionActivities act) => act.TranslateTermAsync(
                new TranslationActivityInput("goodbye", input.LanguageCode.ToLower())),
            activityOptions);

        return new TranslationWorkflowOutput(
            $"{helloResult.Translation}, {input.Name}",
            $"{goodbyeResult.Translation}, {input.Name}");
    }
}