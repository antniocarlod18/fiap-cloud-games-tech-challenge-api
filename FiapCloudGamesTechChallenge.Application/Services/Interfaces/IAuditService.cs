using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Application.Services.Interfaces;

public interface IAuditService
{
    Task<IList<AuditGameUserCollectionResponseDto>> GetByUserAsync(Guid userId, string? collection);
    Task<IList<AuditGameUserCollectionResponseDto>> GetByGameAsync(Guid gameId, string? collection);
    Task<IList<AuditGamePriceResponseDto>> GetByGameAsync(Guid gameId);
}