using Temporalio.DebugActivity.Workflow.Models;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace TemporalioDebugActivity;

[Workflow]
public class PizzaWorkflow
{
    [WorkflowRun]
    public async Task<OrderConfirmation> RunAsync(PizzaOrder order)
    {
        var options = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromSeconds(5),
            RetryPolicy = new() { MaximumInterval = TimeSpan.FromSeconds(10) },
        };

        var totalPrice = order.Items.Sum(pizza => pizza.Price);

        var distance = await Workflow.ExecuteActivityAsync((Activities act) => act.GetDistanceAsync(order.Address), options);

        if (order.IsDelivery && distance.Kilometers > 25)
        {
            throw new ApplicationFailureException("customer lives too far away for delivery");
        }

        // We use a short Timer duration here to avoid delaying the exercise
        await Workflow.DelayAsync(TimeSpan.FromSeconds(3));

        var bill = new Bill(
            CustomerId: order.Customer.CustomerId,
            OrderNumber: order.OrderNumber,
            Description: "Pizza",
            Amount: totalPrice);

        var confirmation = await Workflow.ExecuteActivityAsync((Activities act) => act.SendBillAsync(bill), options);

        return confirmation;
    }
}