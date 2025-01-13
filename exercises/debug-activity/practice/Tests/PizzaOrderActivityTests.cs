using System.Collections.ObjectModel;
using Temporalio.DebugActivity.Workflow.Models;
using Temporalio.Testing;
using TemporalioDebugActivity;
using Xunit;

namespace TemporalioDebugActivity.Tests;
public class PizzaOrderActivityTests
{
    [Fact]
    public async Task TestGetDistanceTwoLineAddressAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Address(
            Line1: "701 Mission Street",
            City: "San Francisco",
            State: "CA",
            PostalCode: "94103",
            Line2: "Apartment 9C");

        var result = await env.RunAsync(() => activities.GetDistanceAsync(input));

        Assert.Equal(20, result.Kilometers);
    }

    [Fact]
    public async Task TestGetDistanceOneLineAddressAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Bill(
            CustomerId: 12983,
            OrderNumber: "PI314",
            Description: "2 large cheese pizzas",
            Amount: 2600); // amount does not qualify for discount

        var result = await env.RunAsync(() => activities.GetDistanceAsync(input));
        Assert.Equal(8, result.Kilometers);
    }

    [Fact]
    public async Task TestSendBillTypicalOrderAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Bill(
            CustomerId: 12983,
            OrderNumber: "PI314",
            Description: "5 large cheese pizzas",
            Amount: 6500); // amount qualifies for discount

        var result = await env.RunAsync(() => activities.SendBillAsync(input));

        Assert.Equal("PI314", result.OrderNumber);
        Assert.Equal(2600, result.Amount);
    }

    // TODO: Write the TestSendBillAppliesDiscountAsync Test
    [Fact]
    public async Task TestSendBillFailsWithNegativeAmountAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Bill(
            CustomerId: 21974,
            OrderNumber: "OU812",
            Description: "1 large sausage pizza",
            Amount: -1000);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => env.RunAsync(() => activities.SendBillAsync(input)));
        Assert.Equal($"INVALID CHARGE AMOUNT: {input.Amount} (MUST BE ABOVE ZERO)", exception.Message);
    }
}