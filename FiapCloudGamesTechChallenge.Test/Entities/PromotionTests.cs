using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Entities;

public class PromotionTests
{
    private Game CreateGame() =>
        new("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 100, "343", "MS", "1.0", true);

    [Fact]
    public void Active_ShouldReturnTrue_WhenWithinDateRange()
    {
        var promo = new Promotion(new List<Game>(), 10, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        Assert.True(promo.Active);
    }

    [Fact]
    public void AddGame_ShouldAddGame_WhenNotAlreadyInList()
    {
        var game = CreateGame();
        var promo = new Promotion(new List<Game>(), 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        promo.AddGame(game);

        Assert.Contains(game, promo.Games);
    }

    [Fact]
    public void AddGame_ShouldThrow_WhenGameAlreadyInPromotion()
    {
        var game = CreateGame();
        var promo = new Promotion(new List<Game> { game }, 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.Throws<InvalidOperationAddingGameToCartException>(() => promo.AddGame(game));
    }

    [Fact]
    public void RemoveGame_ShouldRemove_WhenExists()
    {
        var game = CreateGame();
        var promo = new Promotion(new List<Game> { game }, 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        promo.RemoveGame(game);

        Assert.DoesNotContain(game, promo.Games);
    }
}
