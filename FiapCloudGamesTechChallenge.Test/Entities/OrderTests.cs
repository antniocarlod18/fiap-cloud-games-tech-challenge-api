using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Entities;

public class OrderTests
{
    private Game CreateGame() =>
        new("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 100, "343", "MS", "1.0", true);

    private User CreateUserWithGame()
    {
        var user = new User("John", "hash", "john@x.com");
        user.AddToCart(CreateGame());
        return user;
    }

    [Fact]
    public void Constructor_ShouldPopulateGamesAndSetStatusWaiting()
    {
        var user = CreateUserWithGame();

        var order = new Order(user);

        Assert.NotEmpty(order.Games);
        Assert.Equal(OrderStatusEnum.WaitingForPayment, order.Status);
        Assert.Equal(0, user.GameCart.Count); 
    }

    [Fact]
    public void CompletedOrder_ShouldChangeStatusAndAddGamesToLibrary()
    {
        var user = CreateUserWithGame();
        var order = new Order(user);

        order.CompletedOrder();

        Assert.Equal(OrderStatusEnum.Completed, order.Status);
        Assert.NotEmpty(user.GameLibrary);
    }

    [Fact]
    public void CancelOrder_ShouldChangeStatusAndReturnGamesToCart()
    {
        var user = CreateUserWithGame();
        var order = new Order(user);

        order.CancelOrder();

        Assert.Equal(OrderStatusEnum.Canceled, order.Status);
        Assert.NotEmpty(user.GameCart);
    }

    [Fact]
    public void RefundOrder_ShouldChangeStatusAndRemoveGamesFromLibrary_WhenWithinRefundPeriod()
    {
        var user = CreateUserWithGame();
        var order = new Order(user);
        order.CompletedOrder();

        order.RefundOrder();

        Assert.Equal(OrderStatusEnum.Refunded, order.Status);
    }

    [Fact]
    public void RefundOrder_ShouldThrow_WhenOutsideRefundPeriod()
    {
        var user = CreateUserWithGame();
        var order = new Order(user);
        order.CompletedOrder();
        order.DateCreated = DateTime.UtcNow.AddDays(-40); // simulate old order

        Assert.Throws<CannotRefundOrderException>(() => order.RefundOrder());
    }
}
