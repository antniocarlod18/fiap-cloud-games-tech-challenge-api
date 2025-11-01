using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Entities;

public class AuditGameUserCollectionTests
{
    private Game CreateGame() =>
        new("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 100, "343", "MS", "1.0", true);

    private User CreateUser() =>
        new("John", "hash", "john@x.com");

    [Fact]
    public void AuditGameUserCollection_ShouldStoreAllValuesCorrectly()
    {
        var game = CreateGame();
        var user = CreateUser();

        var audit = new AuditGameUserCollection(
            user,
            game,
            AuditGameUserActionEnum.Added,
            AuditGameUserCollectionEnum.Cart,
            "Added for testing"
        );

        Assert.Equal(user, audit.User);
        Assert.Equal(game, audit.Game);
        Assert.Equal(AuditGameUserActionEnum.Added, audit.Action);
        Assert.Equal(AuditGameUserCollectionEnum.Cart, audit.Collection);
        Assert.Equal("Added for testing", audit.Notes);
    }
}
