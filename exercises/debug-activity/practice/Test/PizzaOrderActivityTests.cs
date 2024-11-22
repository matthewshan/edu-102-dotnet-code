using System.Collections.ObjectModel;
using Temporalio.Testing;
using TemporalioDebugActivity;
using Xunit;

namespace Test;

public class PizzaOrderActivityTests
{
    [Fact]
    public async Task TestGetDistanceTwoLineAddressAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Address
        {
            Line1 = "701 Mission Street",
            Line2 = "Apartment 9C",
            City = "San Francisco",
            State = "CA",
            PostalCode = "94103",
        };

        var result = await env.RunAsync(() => activities.GetDistanceAsync(input));

        Assert.Equal(20, result.Kilometers);
    }

    [Fact]
    public async Task TestGetDistanceOneLineAddressAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Address
        {
            Line1 = "917 Delores Street",
            City = "San Francisco",
            State = "CA",
            PostalCode = "94103",
        };
        var result = await env.RunAsync(() => activities.GetDistanceAsync(input));
        Assert.Equal(8, result.Kilometers);
    }

    [Fact]
    public async Task TestSendBillTypicalOrderAsync()
    {
        var env = new ActivityEnvironment();
        var activities = new Activities();
        var input = new Bill
        {
            CustomerId = 12983,
            OrderNumber = "PI314",
            Description = "2 large cheese pizzas",
            Amount = 2600, // amount does not qualify for discount
        };

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
        var input = new Bill
        {
            CustomerId = 21974,
            OrderNumber = "OU812",
            Description = "1 large sausage pizza",
            Amount = -1000,
        };

        async Task<OrderConfirmation> ActAsync() => await env.RunAsync(() => activities.SendBillAsync(input));

        var exception = await Assert.ThrowsAsync<ArgumentException>(ActAsync);
        Assert.Equal("invalid charge amount: -1000 (must be above zero)", exception.Message);
    }
}