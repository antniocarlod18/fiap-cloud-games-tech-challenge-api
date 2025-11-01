using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Application.Services.Interfaces;

public interface IPromotionService
{
    Task<PromotionResponseDto?> AddAsync(PromotionRequestDto dto);
    Task<PromotionResponseDto?> GetAsync(Guid id);
    Task<IList<PromotionResponseDto>> GetActiveAsync();
    Task<PromotionResponseDto?> DeleteAsync(Guid id);
    Task<PromotionResponseDto?> AddGameToPromotionAsync(Guid id, Guid gameId);
    Task<PromotionResponseDto?> RemoveGameToPromotionAsync(Guid id, Guid gameId);
}