namespace Temporalio.DebugActivity.Workflow.Models;

public record Customer(
    int CustomerId,
    string Name,
    string Email = "",
    string Phone);