using Temporalio.DebugActivity.Workflow.Models;

namespace TemporalioDebugActivity;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    List<Pizza>? Items,
    bool IsDelivery = false,
    Address? Address);