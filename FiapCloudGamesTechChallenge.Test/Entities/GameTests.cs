using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Entities;

public class GameTests
{
    private Game CreateGame(decimal price = 100)
    {
        return new Game("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", price, "343", "MS", "1.0", true);
    }

    [Fact]
    public void UpdatePrice_ShouldChangePriceAndDateUpdated()
    {
        var game = CreateGame();
        var oldDate = DateTime.UtcNow;

        game.UpdatePrice(200);

        Assert.Equal(200, game.Price);
        Assert.True(game.DateUpdated > oldDate);
    }

    [Fact]
    public void GetPriceWithDiscount_ShouldReturnPrice_WhenNoPromotion()
    {
        var game = CreateGame(100);
        var price = game.GetPriceWithDiscount();

        Assert.Equal(100, price);
    }

    [Fact]
    public void GetPriceWithDiscount_ShouldReturnDiscountedPrice_WhenActivePromotionExists()
    {
        var game = CreateGame(100);
        var promo = new Promotion(new List<Game> { game }, 20, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        game.Promotions = new List<Promotion> { promo };

        var price = game.GetPriceWithDiscount();

        Assert.Equal(80, price);
    }
}