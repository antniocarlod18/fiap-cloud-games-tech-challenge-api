using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;

namespace FiapCloudGamesTechChallenge.Application.Dtos;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IList<Guid> Games { get; set; } = [];
    public decimal Price { get; set; }
    public string Status { get; set; } = "";
    public DateTime DateCreated { get; set; }

    public static implicit operator OrderResponseDto?(Order? order)
    {
        if (order == null) return null;
        return new OrderResponseDto
        {
            Id = order.Id,
            UserId = order.User.Id,
            Games = order.Games.Select(g => g.Game.Id).ToList(),
            Price = order.Price,
            Status = order.Status.ToString(),
            DateCreated = order.DateCreated
        };
    }
}