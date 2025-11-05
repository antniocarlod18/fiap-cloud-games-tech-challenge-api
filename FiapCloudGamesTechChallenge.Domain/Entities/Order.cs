using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesTechChallenge.Domain.Entities;

public class Order : EntityBase
{
    const int RefundPeriodInDays = 30;

    public required User User { get; set; }
    public required IList<OrderGameItem> Games { get; set; }
    public decimal Price { get; private set; }
    public required OrderStatusEnum Status { get; set; }

    [SetsRequiredMembers]
    public Order(User user) : base()
    {
        if(!user.GameCart.Any()) 
            throw new CannotCreateAnOrderWithoutItemsException(); 

        User = user;

        Games = new List<OrderGameItem>();
        foreach (var game in user.GameCart)
        {
            Games.Add(new OrderGameItem(this, game));
        }

        user.ClearCart();

        Status = OrderStatusEnum.WaitingForPayment;
        Price = Games.Sum(g => g.Price);
    }

    [SetsRequiredMembers]
    public Order()
    {
    }

    public void CompletedOrder()
    {
        if(Status != OrderStatusEnum.WaitingForPayment)
            throw new InvalidOrderStatusException(Status);

        Status = OrderStatusEnum.Completed;
        User.PurchaseGames(Games.Select(g => g.Game).ToList());
        DateUpdated = DateTime.UtcNow;
    }

    public void CancelOrder()
    {
        if (Status != OrderStatusEnum.WaitingForPayment)
            throw new InvalidOrderStatusException(Status);

        Status = OrderStatusEnum.Canceled;
        User.ReturnGamesToCart(Games.Select(g => g.Game).ToList());
        DateUpdated = DateTime.UtcNow;
    }

    public void RefundOrder()
    {
        if(Status == OrderStatusEnum.Completed && DateTime.Now < base.DateCreated.AddDays(RefundPeriodInDays))
        {
            Status = OrderStatusEnum.Refunded;
            User.RefundGames(Games.Select(g => g.Game).ToList());
            DateUpdated = DateTime.UtcNow;
        }
        else
        {
            throw new CannotRefundOrderException();
        }
    }
}
