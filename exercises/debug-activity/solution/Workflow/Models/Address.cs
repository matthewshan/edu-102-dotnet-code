Temporalio.DebugActivity.Workflow.Models;

public record Address(
    string Line1,
    string? Line2 = null,
    string City,
    string State,
    string PostalCode);