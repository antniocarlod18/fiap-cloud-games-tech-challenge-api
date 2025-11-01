using FiapCloudGamesTechChallenge.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesTechChallenge.Domain.Entities;

public class Promotion : EntityBase
{
    public IList<Game> Games { get; set; } = [];
    public required decimal DiscountPercentage { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public bool Active { 
        get
        {
            return DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
        }
    }

    [SetsRequiredMembers]
    public Promotion(IList<Game> gamePromotions, decimal discountPercentage, DateTime startDate, DateTime endDate) : base()
    {
        Games = gamePromotions;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
    }

    [SetsRequiredMembers]
    protected Promotion()
    {
    }

    public void AddGame(Game game)
    {
        if (Games.Contains(game))
            throw new InvalidOperationAddingGameToCartException(); //TODO custom exception

        Games.Add(game);
    }

    public void RemoveGame(Game game)
    {
        if (Games.Contains(game))
        {
            Games.Remove(game);
        }
    }
}
