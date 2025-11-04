using FiapCloudGamesTechChallenge.Application.Dtos;

namespace FiapCloudGamesTechChallenge.Application.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto?> AddAsync(OrderRequestDto dto);
    Task<OrderDetailedResponseDto?> GetAsync(Guid id);
    Task<IList<OrderResponseDto?>> GetByUserAsync(Guid idUser);
    Task<OrderDetailedResponseDto?> CancelOrderAsync(Guid orderId, Guid idUser);
    Task<OrderDetailedResponseDto?> CompleteOrderAsync(Guid orderId, Guid idUser);
    Task<OrderDetailedResponseDto?> RefundOrderAsync(Guid orderId, Guid idUser);
}