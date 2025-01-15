namespace TemporalioDebugActivity.Tests;

using Temporalio.DebugActivity.Workflow.Models;
using Temporalio.Testing;
using TemporalioDebugActivity;
using Xunit;

public class PizzaOrderActivityTests
{
    [Fact]
    public async Task GetDistanceTwoLineAddressAsync()
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
    public async Task GetDistanceOneLineAddressAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Address(
            Line1: "917 Delores Street",
            City: "San Francisco",
            State: "CA",
            PostalCode: "94103");

        var result = await env.RunAsync(() => activities.GetDistanceAsync(input));
        Assert.Equal(8, result.Kilometers);
    }

    [Fact]
    public async Task SendBillTypicalOrderAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Bill(
            CustomerId: 12983,
            OrderNumber: "PI314",
            Description: "2 large cheese pizzas",
            Amount: 2600); // amount does not qualify for discount

        var result = await env.RunAsync(() => activities.SendBillAsync(input));

        Assert.Equal("PI314", result.OrderNumber);
        Assert.Equal(2600, result.Amount);
    }

    [Fact]
    public async Task SendBillAppliesDiscountAsync()
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
        Assert.Equal(6000, result.Amount);
    }

    [Fact]
    public async Task SendBillFailsWithNegativeAmountAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Bill(
            CustomerId: 21974,
            OrderNumber: "OU812",
            Description: "1 large sausage pizza",
            Amount: -1000);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => env.RunAsync(() => activities.SendBillAsync(input)));
        Assert.Equal($"Invalid charge amount: {input.Amount} (must be above zero)", exception.Message);
    }
}