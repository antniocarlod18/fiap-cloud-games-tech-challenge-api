using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Entities;

public class AuditGamePriceTests
{
    private Game CreateGame() =>
        new("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 100, "343", "MS", "1.0", true);

    [Fact]
    public void AuditGamePrice_ShouldStoreAllValuesCorrectly()
    {
        var game = CreateGame();
        var audit = new AuditGamePrice(game, 100, 120, "Seasonal update");

        Assert.Equal(game, audit.Game);
        Assert.Equal(100, audit.OldPrice);
        Assert.Equal(120, audit.NewPrice);
        Assert.Equal("Seasonal update", audit.Justification);
    }
}
