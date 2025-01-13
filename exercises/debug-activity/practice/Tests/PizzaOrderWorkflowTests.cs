using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.DebugActivity.Workflow.Models;
using Temporalio.Exceptions;
using Temporalio.Testing;
using Temporalio.Worker;
using Temporalio.Workflows;
using TemporalioDebugActivity;
using Xunit;

namespace TemporalioDebugActivity.Tests;

public class PizzaOrderWorkflowTests
{
    [Fact]
    public async Task TestSuccessfulPizzaOrderAsync()
    {
        var taskQueueId = Guid.NewGuid().ToString();
        var order = CreatePizzaOrderForTest();

        // For this test, any address will have a distance of 10 kilometer, which
        // is within the delivery area
        [Activity("GetDistance")]
        Task<Distance> MockDistanceActivityAsync(Address addr) =>
            Task.FromResult(new Distance(10));

        [Activity("SendBill")]
        Task<OrderConfirmation> MockBillActivityAsync(Bill bill) =>
            Task.FromResult(
                new OrderConfirmation(
                OrderNumber: order.OrderNumber,
                Status: "SUCCESS",
                ConfirmationNumber: "AB9923",
                BillingTimestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Amount: 2500));

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync(new()
        {
            LoggerFactory = loggerFactory,
        });

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueueId)
                .AddWorkflow<PizzaWorkflow>()
                .AddActivity(MockBillActivityAsync)
                .AddActivity(MockDistanceActivityAsync));

        var result = await worker.ExecuteAsync(
            () => env.Client.ExecuteWorkflowAsync((PizzaWorkflow wf) => wf.RunAsync(order), new WorkflowOptions(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId)));

        Assert.Equal("Z1238", result.OrderNumber);
        Assert.Equal("SUCCESS", result.Status);
        Assert.Equal("AB9923", result.ConfirmationNumber);
        Assert.Equal(2500, result.Amount);
        Assert.True(result.BillingTimestamp > 0);
    }

    [Fact]
    public async Task TestFailedPizzaOrderCustomerOutsideDeliveryAreaAsync()
    {
        var taskQueueId = Guid.NewGuid().ToString();
        var order = CreatePizzaOrderForTest();

        [Activity("GetDistance")]
        Task<Distance> MockDistanceActivityAsync(Address addr) =>
            Task.FromResult(new Distance { Kilometers = 30, });

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync(new()
        {
            LoggerFactory = loggerFactory,
        });

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueueId)
                .AddWorkflow<PizzaWorkflow>()
                .AddActivity(MockDistanceActivityAsync));

        async Task<OrderConfirmation> ActAsync() =>
            await worker.ExecuteAsync(
                async () =>
                    await env.Client.ExecuteWorkflowAsync(
                        (PizzaWorkflow wf) => wf.RunAsync(order),
                        new WorkflowOptions(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId)));

        // Since the Workflow failed, trying to access its result fails
        var exception = await Assert.ThrowsAsync<WorkflowFailedException>(ActAsync);

        // When the Workflow returns an error during its execution, Temporal
        // wraps it in a Temporal-specific WorkflowExecutionError type, so we
        // must unwrap this to retrieve the error returned in the Workflow code.
        var inner = exception.InnerException as ApplicationFailureException;
        Assert.Equal("customer lives too far away for delivery", inner?.Failure?.Message);
    }

    private static PizzaOrder CreatePizzaOrderForTest()
    {
        var customer = new Customer(
        CustomerId: 12983,
        Name: "María García",
        Phone: "415-555-7418",
        Email: "maria1985@example.com");

        var address = new Address(
        Line1: "701 Mission Street",
        City: "San Francisco",
        State: "CA",
        PostalCode: "94103",
        Line2: "Apartment 9C");

        var p1 = new Pizza(
        Description: "Large, with pepperoni",
        Price: 1500);
        var p2 = new Pizza(
            Description: "Small, with mushrooms and onions",
            Price: 1000);

        var pizzaList = new List<Pizza> { p1, p2 };
        var pizzas = new Collection<Pizza>(pizzaList);

        return new PizzaOrder(
        OrderNumber: "Z1238",
        Customer: customer,
        Items: pizzas,
        Address: address,
        IsDelivery: true);
    }
}