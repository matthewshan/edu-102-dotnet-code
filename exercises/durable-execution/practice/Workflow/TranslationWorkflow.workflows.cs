namespace TemporalioDurableExecution.Workflow;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Temporalio.Workflows;

[Workflow]
public class TranslationWorkflow
{
    public record TranslationWorkflowInput(string Name, string LanguageCode);

    public record TranslationWorkflowOutput(string HelloMessage, string GoodbyeMessage);

    [WorkflowRun]
    public async Task<TranslationWorkflowOutput> RunAsync(TranslationWorkflowInput input)
    {
        // TODO Part A: Define the Workflow logger here
        var logger = Workflow.Logger;

        // TODO Part A: At the Info level, log that the Workflow function has been invoked.
        // Include the name passed as input.
        logger.LogInformation("Hello, my name is '{Name}'", input.Name);

        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        // TODO Part A: At the Information level, log a message stating that the translation will happen now
        // Include the language code passed as input
        var input1 = new Activities.TranslateTermInput("hello", input.LanguageCode.ToLower());
        logger.LogInformation("Executing TranslateTermAsync Activity with input record: {Input}", JsonSerializer.Serialize(input1));
        var helloResult = await Workflow.ExecuteActivityAsync(
            (Activities act) => act.TranslateTermAsync(input1),
            activityOptions);

        // TODO Part C: At the Information level, log the message: "Sleeping between translation calls"
        // TODO Part C: Use `Workflow.DelayAsync` to set a Timer for 10 seconds.
        logger.LogInformation("Sleeping between translation calls");
        await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

        // TODO Part A: At the Information level, log a message stating that the translation will happen now
        // Include the language code passed as input
        var input2 = new Activities.TranslateTermInput("goodbye", input.LanguageCode.ToLower());
        logger.LogInformation("Executing TranslateTermAsync Activity with input record: {Input}", JsonSerializer.Serialize(input2));
        var goodbyeResult = await Workflow.ExecuteActivityAsync(
            (Activities act) => act.TranslateTermAsync(input2),
            activityOptions);

        return new TranslationWorkflowOutput(
            $"{helloResult.Translation}, {input.Name}",
            $"{goodbyeResult.Translation}, {input.Name}");
    }
}