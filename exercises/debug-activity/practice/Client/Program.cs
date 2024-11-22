// This file is designated to run the Workflow
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Temporalio.Client;
using TemporalioDebugActivity;

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new()
{
    TargetHost = "localhost:7233",
});

var order = CreatePizzaOrder();

// Run workflow
var result = await client.ExecuteWorkflowAsync(
    (PizzaWorkflow wf) => wf.RunAsync(order),
    new WorkflowOptions
    {
        Id = $"pizza-workflow-order-{order.OrderNumber}",
        TaskQueue = WorkflowConstants.TaskQueueName,
    });

Console.WriteLine($"""
Workflow result:
  Order Number: {result.OrderNumber}
  Status: {result.Status}
  Confirmation Number: {result.ConfirmationNumber}
  Billing Timestamp: {DateTimeOffset.FromUnixTimeSeconds(result.BillingTimestamp).UtcDateTime}
  Amount: ${result.Amount / 100.0:F2}
""");

PizzaOrder CreatePizzaOrder()
{
    var customer = new Customer
    {
        CustomerId = 12983,
        Name = "María García",
        Email = "maria1985@example.com",
        Phone = "415-555-7418",
    };

    var address = new Address
    {
        Line1 = "701 Mission Street",
        Line2 = "Apartment 9C",
        City = "San Francisco",
        State = "CA",
        PostalCode = "94103",
    };

    var p1 = new Pizza { Description = "Large, with mushrooms and onions", Price = 1500 };
    var p2 = new Pizza { Description = "Small, with pepperoni", Price = 1200 };
    // TODO Part C: Define an additional pizza

    // TODO Part C: Add the additional pizza to the pizza list
    var pizzaList = new List<Pizza> { p1, p2 };

    var pizzas = new Collection<Pizza>(pizzaList);

    return new PizzaOrder
    {
        OrderNumber = "Z1238",
        Customer = customer,
        Items = pizzas,
        Address = address,
        IsDelivery = true,
    };
}