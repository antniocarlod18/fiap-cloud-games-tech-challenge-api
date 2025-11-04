using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FiapCloudGamesTechChallenge.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
    }

    public async Task<UserResponseDto?> AddAsync(UserRequestDto dto)
    {
        var passwordRandom = GenerateRandomPassword();
        _logger.LogDebug("Password: " + passwordRandom);
        var hashedPassword = HashPassword(passwordRandom);

        var user = new User(dto.Name, hashedPassword, dto.Email);

        await _unitOfWork.UsersRepo.AddAsync(user);
        await _unitOfWork.Commit();
        return user;
    }

    public async Task<UserDetailedResponseDto?> GetAsync(Guid id)
    {
        var user = await _unitOfWork.UsersRepo.GetDetailedByIdAsync(id);

        if(user == null)
            throw new ResourceNotFoundException(nameof(User));

        return user;
    }

    public async Task<UserResponseDto?> UnlockAsync(Guid id, UserUnlockRequestDto userUnlockRequestDto)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
            throw new ResourceNotFoundException(nameof(User));

        var hashedPassword = HashPassword(userUnlockRequestDto.Password);
        user.UnlockAccount(hashedPassword);
        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        return user;
    }

    public async Task<UserResponseDto?> MakeAdminAsync(Guid id)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
            throw new ResourceNotFoundException(nameof(User));
        
        user.MakeAdmin();
        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        return user;
    }

    public async Task<UserResponseDto?> RevokeAdminAsync(Guid id)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
            throw new ResourceNotFoundException(nameof(User));

        user.RevokeAdmin();
        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        return user;
    }

    public async Task<IList<UserResponseDto?>> GetAllAsync()
    {
        var users = await _unitOfWork.UsersRepo.GetAllAsync();
        return users.Select(x => (UserResponseDto?)x).ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
            throw new ResourceNotFoundException(nameof(User)); 

        user.LockUser();

        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateRequestDto dto)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);
        
        if (user == null)
            throw new ResourceNotFoundException(nameof(User));

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.HashPassword = HashPassword(dto.Password);
        user.DateUpdated = DateTime.UtcNow;

        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
        return user;
    }

    public async Task<UserResponseDto?> AuthenticateAsync(UserAuthenticateRequestDto dto)
    {
        var user = await _unitOfWork.UsersRepo.GetByEmailAsync(dto.Email);

        if (user == null)
            throw new AuthorizationException();

        if(!VerifyPassword(user.HashPassword, dto.Password))
            throw new AuthorizationException();
        
        return user;
    }

    public async Task AddGameToCart(Guid id, Guid gameId)
    {
        var user = await _unitOfWork.UsersRepo.GetDetailedByIdAsync(id);
        if (user == null)
            throw new ResourceNotFoundException(nameof(User)); 

        var game = await _unitOfWork.GamesRepo.GetWithPromotionsByIdAsync(gameId);
        if (game == null)
            throw new ResourceNotFoundException(nameof(User));

        user.AddToCart(game);

        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.AuditGameUsersRepo.AddAsync(
            new AuditGameUserCollection(
                user,
                game,
                Domain.Enums.AuditGameUserActionEnum.Added,
                Domain.Enums.AuditGameUserCollectionEnum.Cart,
                "Game added to cart"));
        await _unitOfWork.Commit();
    }

    public async Task RemoveGameFromCart(Guid id, Guid gameId)
    {
        var user = await _unitOfWork.UsersRepo.GetDetailedByIdAsync(id);
        if (user == null)
            throw new ResourceNotFoundException(nameof(User));

        var game = await _unitOfWork.GamesRepo.GetWithPromotionsByIdAsync(gameId);
        if (game == null)
            throw new ResourceNotFoundException(nameof(User));

        user.RemoveFromCart(game);

        _unitOfWork.GamesRepo.Attach(game);
        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.AuditGameUsersRepo.AddAsync(
            new AuditGameUserCollection(
                user, 
                game, 
                Domain.Enums.AuditGameUserActionEnum.Removed,
                Domain.Enums.AuditGameUserCollectionEnum.Cart,
                "Game removed from cart"));

        await _unitOfWork.Commit();
    }

    private string GenerateRandomPassword()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
    }

    private string HashPassword(string password)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(plainTextBytes).ToString();
    }

    private bool VerifyPassword(string hashedPassword, string password)
    {
        var hashedInputPassword = HashPassword(password);
        return hashedPassword == hashedInputPassword;
    }
}