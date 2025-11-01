using FiapCloudGamesTechChallenge.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesTechChallenge.Domain.Entities;

public class User : EntityBase
{
    public required string Name { get; set; }
    public required string HashPassword { get; set; }
    public required string Email { get; set; }
    public required bool Active { get; set; }
    public required bool IsAdmin { get; set; }
    public IList<Game> GameLibrary { get; set; } = [];
    public IList<Game> GameCart { get; set; } = [];
    public IList<Order> Orders { get; set; } = [];

    [SetsRequiredMembers]
    public User(string name, string hashPassword, string email) : base()
    {
        Name = name;
        HashPassword = hashPassword;
        Email = email;
        Active = false;
    }

    [SetsRequiredMembers]
    protected User()
    {
    }

    public void UnlockAccount(string hashPassword)
    {
        HashPassword = hashPassword;
        Active = true;
        DateUpdated = DateTime.UtcNow;
    }

    public void LockUser()
    {
        Active = false;
        DateUpdated = DateTime.UtcNow;
    }

    public void MakeAdmin()
    {
        IsAdmin = true;
        DateUpdated = DateTime.UtcNow;
    }

    public void RevokeAdmin()
    {
        IsAdmin = false;
        DateUpdated = DateTime.UtcNow;
    }

    public void AddToCart(Game game)
    {
        if (GameLibrary.Contains(game) || GameCart.Contains(game))
            throw new InvalidOperationAddingGameToCartException();

        GameCart.Add(game);       
    }

    public void RemoveFromCart(Game game)
    {
        if (GameCart.Contains(game))
        {
            GameCart.Remove(game);
        }
    }

    public void ClearCart()
    {
        GameCart.Clear();
    }

    public void PurchaseGames(IList<Game> games)
    {
        foreach (var game in games)
        {
            if (!GameLibrary.Contains(game))
            {
                GameLibrary.Add(game);
            }
        }
    }

    public void ReturnGamesToCart(IList<Game> games)
    {
        foreach (var game in games)
        {
            if (!GameLibrary.Contains(game) && !GameCart.Contains(game))
            {
                GameCart.Add(game);
            }
        }
    }

    public void RefundGames(IList<Game> games)
    {
        foreach (var game in games)
        {
            if (GameLibrary.Contains(game))
            {
                GameLibrary.Remove(game);
            }
        }
    }
}
