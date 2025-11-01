using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Application.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto?> AddAsync(OrderRequestDto dto);
    Task<OrderDetailedResponseDto?> GetAsync(Guid id);
    Task<IList<OrderResponseDto>> GetByUserAsync(Guid idUser);
}