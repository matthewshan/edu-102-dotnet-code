namespace TemporalioUsingRecords;

using Temporalio.Workflows;

public record TranslationWorkflowInput(string Name, string LanguageCode);
public record TranslationWorkflowOutput(string HelloMessage, string GoodbyeMessage);

[Workflow]
public class TranslationWorkflow
{
    [WorkflowRun]
    public async Task<TranslationWorkflowOutput> RunAsync(TranslationWorkflowInput input)
    {
        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        var helloInput = new TranslationActivityInput("Hello", input.LanguageCode);

        var helloResult =
            await Workflow.ExecuteActivityAsync((Activities act) => act.TranslateTermAsync(helloInput), activityOptions);

        var helloMessage = $"{helloResult.Translation}, {input.Name}";

        await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

        var goodbyeInput = new TranslationActivityInput("Goodbye", input.LanguageCode);

        var byeMessage =
            await Workflow.ExecuteActivityAsync((Activities act) => act.TranslateTermAsync(goodbyeInput), activityOptions);

        var goodbyeMessage = $"{byeMessage.Translation}, {input.Name}";

        return new TranslationWorkflowOutput(helloMessage, goodbyeMessage);
    }
}