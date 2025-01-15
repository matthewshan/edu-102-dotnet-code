namespace Temporalio.DebugActivity.Workflow.Models;

public record Customer(
    int CustomerId,
    string Name,
    string Phone,
    string Email = "");