namespace TemporalioDurableExecution.Workflow;

using Microsoft.Extensions.Logging;
using Temporalio.Workflows;


[Workflow]
public class TranslationWorkflow
{
    public record TranslationWorkflowInput(string Name, string LanguageCode);
    public record TranslationWorkflowOutput(string HelloMessage, string GoodbyeMessage);

    [WorkflowRun]
    public async Task<TranslationWorkflowInput> RunAsync(TranslationWorkflowInput input)
    {
        var logger = Workflow.Logger;

        logger.LogInformation("TranslationWorkflow invoked with name {Name}", input.Name);

        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        logger.LogInformation("Preparing to translate 'Hello', '{LanguageCode}'", input.LanguageCode);
        var helloResult = await Workflow.ExecuteActivityAsync(
           (Activities act) => act.TranslateTermAsync(
               new TranslateTermInput("hello", input.LanguageCode.ToLower())),
           activityOptions);

        logger.LogInformation("Sleeping between translation calls");
        await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

        logger.LogInformation("Preparing to translate 'Goodbye', '{LanguageCode}'", input.LanguageCode);
        var goodbyeResult = await Workflow.ExecuteActivityAsync(
            (Activities act) => act.TranslateTermAsync(
                new TranslateTermInput("goodbye", input.LanguageCode.ToLower())),
            activityOptions);

        return new TranslationWorkflowOutput(
            $"{helloResult.Translation}, {input.Name}",
            $"{goodbyeResult.Translation}, {input.Name}");
    }
}