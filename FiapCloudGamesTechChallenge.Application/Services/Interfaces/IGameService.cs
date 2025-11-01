using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Application.Services.Interfaces;

public interface IGameService
{
    Task<GameResponseDto?> AddAsync(GameRequestDto gameRequestDto);
    Task<GameResponseDto?> GetAsync(Guid id);
    Task<IList<GameResponseDto?>> GetAllAvailableAsync();
    Task<GameResponseDto> GetByTitleAsync(string title);
    Task<IList<GameResponseDto?>> GetByGenreAsync(string genre);
    Task<GameResponseDto?> UpdateAsync(Guid id, GameUpdateRequestDto gameRequestDto);
}
