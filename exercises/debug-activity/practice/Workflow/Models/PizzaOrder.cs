using System.Collections.ObjectModel;

namespace Temporalio.DebugActivity.Workflow.Models;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    List<Pizza>? Items,
    bool IsDelivery = false,
    Address? Address);