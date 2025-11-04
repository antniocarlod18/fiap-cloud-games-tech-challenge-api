using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Application.Dtos;

public class UserDetailedResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Active { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public IList<OrderResponseDto> Orders { get; set; } = [];
    public IList<GameResponseDto> GameLibrary { get; set; } = [];
    public IList<GameResponseDto> GameCart { get; set; } = [];


    public static implicit operator UserDetailedResponseDto?(User? user)
    {
        if (user == null) return null;
        return new UserDetailedResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Active = user.Active,
            IsAdmin = user.IsAdmin,
            DateCreated = user.DateCreated,
            DateUpdated = user.DateUpdated,
            Orders = user.Orders.Select(o => (OrderResponseDto)o!).ToList(),
            GameLibrary = user.GameLibrary.Select(g => (GameResponseDto)g!).ToList(),
            GameCart = user.GameCart.Select(g => (GameResponseDto)g!).ToList()
        };
    }
}