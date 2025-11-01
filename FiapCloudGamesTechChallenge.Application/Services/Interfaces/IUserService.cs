using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> AddAsync(UserRequestDto dto);
    Task<UserResponseDto?> GetAsync(Guid id);
    Task<UserResponseDto?> UnlockAsync(Guid id, UserUnlockRequestDto userUnlockRequestDto);
    Task<UserResponseDto?> MakeAdminAsync(Guid id);
    Task<UserResponseDto?> RevokeAdminAsync(Guid id);
    Task<IList<UserResponseDto?>> GetAllAsync();
    Task DeleteAsync(Guid id);
    Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateRequestDto dto);
    Task<UserResponseDto?> AuthenticateAsync(UserAuthenticateRequestDto dto);
    Task AddGameToCart(Guid id, Guid gameId);
    Task RemoveGameFromCart(Guid id, Guid gameId);

}