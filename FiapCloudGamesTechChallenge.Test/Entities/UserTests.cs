using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Entities;

public class UserTests
{
    private readonly Game _game1;
    private readonly Game _game2;

    public UserTests()
    {
        var platforms = new List<GamePlatformEnum> { GamePlatformEnum.PC, GamePlatformEnum.XboxSeriesX };
        _game1 = new Game("Halo", "FPS", platforms, "Shooter game", 199.90m, "343 Industries", "Microsoft", "1.0", true);
        _game2 = new Game("Forza Horizon", "Racing", platforms, "Car racing", 249.90m, "Playground Games", "Microsoft", "1.0", true);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithInactiveUser()
    {
        // Arrange & Act
        var user = new User("John Doe", "hashed123", "john@example.com");

        // Assert
        Assert.Equal("John Doe", user.Name);
        Assert.Equal("hashed123", user.HashPassword);
        Assert.Equal("john@example.com", user.Email);
        Assert.False(user.Active);
        Assert.Empty(user.GameLibrary);
        Assert.Empty(user.GameCart);
        Assert.Empty(user.Orders);
    }

    [Fact]
    public void UnlockAccount_ShouldActivateUserAndUpdatePassword()
    {
        // Arrange
        var user = new User("John", "oldHash", "john@example.com");
        var oldDate = DateTime.UtcNow;

        // Act
        user.UnlockAccount("newHash");

        // Assert
        Assert.True(user.Active);
        Assert.Equal("newHash", user.HashPassword);
        Assert.True(user.DateUpdated > oldDate);
    }

    [Fact]
    public void LockUser_ShouldDeactivateUserAndUpdateDate()
    {
        // Arrange
        var user = new User("John", "hash", "john@example.com");
        user.UnlockAccount("hash2"); // ativar antes
        var oldDate = user.DateUpdated;

        // Act
        user.LockUser();

        // Assert
        Assert.False(user.Active);
        Assert.True(user.DateUpdated > oldDate);
    }

    [Fact]
    public void MakeAdmin_ShouldSetIsAdminToTrue()
    {
        var user = new User("John", "hash", "john@example.com");

        user.MakeAdmin();

        Assert.True(user.IsAdmin);
        Assert.True(user.DateUpdated <= DateTime.UtcNow);
    }

    [Fact]
    public void RevokeAdmin_ShouldSetIsAdminToFalse()
    {
        var user = new User("John", "hash", "john@example.com");
        user.MakeAdmin();

        user.RevokeAdmin();

        Assert.False(user.IsAdmin);
    }

    [Fact]
    public void AddToCart_ShouldAddGame_WhenNotInLibraryOrCart()
    {
        var user = new User("John", "hash", "john@example.com");

        user.AddToCart(_game1);

        Assert.Single(user.GameCart);
        Assert.Contains(_game1, user.GameCart);
    }

    [Fact]
    public void AddToCart_ShouldThrow_WhenGameAlreadyInLibrary()
    {
        var user = new User("John", "hash", "john@example.com");
        user.GameLibrary.Add(_game1);

        Assert.Throws<InvalidOperationAddingGameToCartException>(() => user.AddToCart(_game1));
    }

    [Fact]
    public void AddToCart_ShouldThrow_WhenGameAlreadyInCart()
    {
        var user = new User("John", "hash", "john@example.com");
        user.AddToCart(_game1);

        Assert.Throws<InvalidOperationAddingGameToCartException>(() => user.AddToCart(_game1));
    }

    [Fact]
    public void RemoveFromCart_ShouldRemoveGame_WhenItExists()
    {
        var user = new User("John", "hash", "john@example.com");
        user.AddToCart(_game1);

        user.RemoveFromCart(_game1);

        Assert.Empty(user.GameCart);
    }

    [Fact]
    public void ClearCart_ShouldRemoveAllGamesFromCart()
    {
        var user = new User("John", "hash", "john@example.com");
        user.AddToCart(_game1);
        user.AddToCart(_game2);

        user.ClearCart();

        Assert.Empty(user.GameCart);
    }

    [Fact]
    public void PurchaseGames_ShouldMoveGamesToLibrary()
    {
        var user = new User("John", "hash", "john@example.com");
        var games = new List<Game> { _game1, _game2 };

        user.PurchaseGames(games);

        Assert.Equal(2, user.GameLibrary.Count);
    }

    [Fact]
    public void ReturnGamesToCart_ShouldAddGamesNotInLibraryOrCart()
    {
        var user = new User("John", "hash", "john@example.com");

        user.ReturnGamesToCart(new List<Game> { _game1 });

        Assert.Single(user.GameCart);
        Assert.Contains(_game1, user.GameCart);
    }

    [Fact]
    public void RefundGames_ShouldRemoveGamesFromLibrary()
    {
        var user = new User("John", "hash", "john@example.com");
        user.PurchaseGames(new List<Game> { _game1 });

        user.RefundGames(new List<Game> { _game1 });

        Assert.Empty(user.GameLibrary);
    }

    [Fact]
    public void RefundGames_ShouldIgnoreGamesNotInLibrary()
    {
        var user = new User("John", "hash", "john@example.com");

        user.RefundGames(new List<Game> { _game1 });

        Assert.Empty(user.GameLibrary);
    }
}
