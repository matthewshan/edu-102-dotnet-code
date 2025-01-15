using System.Collections.ObjectModel;
using Temporalio.DebugActivity.Workflow.Models;

namespace TemporalioDebugActivity;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);