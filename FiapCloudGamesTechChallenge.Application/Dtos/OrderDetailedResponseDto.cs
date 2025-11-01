using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;

namespace FiapCloudGamesTechChallenge.Application.Dtos;

public class OrderDetailedResponseDto
{
    public Guid Id { get; set; }
    public UserResponseDto User { get; set; }
    public IList<OrderGameItemResponseDto> Games { get; set; } = [];
    public decimal Price { get; set; }
    public OrderStatusEnum Status { get; set; }
    public DateTime DateCreated { get; set; }

    public static implicit operator OrderDetailedResponseDto?(Order? order)
    {
        if (order == null) return null;
        
        return new OrderDetailedResponseDto
        {
            Id = order.Id,
            User = order.User,
            Games = (IList<OrderGameItemResponseDto>)order.Games,
            Price = order.Price,
            Status = order.Status,
            DateCreated = order.DateCreated
        };
    }
}