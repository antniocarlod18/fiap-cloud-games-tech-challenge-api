using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using System.Collections.Generic;

namespace FiapCloudGamesTechChallenge.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task<UserResponseDto?> AddAsync(UserRequestDto dto)
    {
        var passwordRandom = GenerateRandomPassword();
        var hashedPassword = HashPassword(passwordRandom);

        var user = new User(dto.Name, hashedPassword, dto.Email);

        await _unitOfWork.UsersRepo.AddAsync(user);
        await _unitOfWork.Commit();
        return user;
    }

    public async Task<UserResponseDto?> GetAsync(Guid id)
    {
        var user = await _unitOfWork.UsersRepo.GetDetailedByIdAsync(id);

        if(user == null)
            throw new ResourceNotFoundException<User>();

        return user;
    }

    public async Task<UserResponseDto?> UnlockAsync(Guid id, UserUnlockRequestDto userUnlockRequestDto)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
            throw new ResourceNotFoundException<User>();

        var hashedPassword = HashPassword(userUnlockRequestDto.Password);
        user.UnlockAccount(hashedPassword);
        _unitOfWork.UsersRepo.Update(user);
        return user;
    }

    public async Task<UserResponseDto?> MakeAdminAsync(Guid id)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
            throw new ResourceNotFoundException<User>();
        
        user.MakeAdmin();
        _unitOfWork.UsersRepo.Update(user);
        return user;
    }

    public async Task<UserResponseDto?> RevokeAdminAsync(Guid id)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);

        if (user == null)
            throw new ResourceNotFoundException<User>();

        user.RevokeAdmin();
        _unitOfWork.UsersRepo.Update(user);
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
            throw new ResourceNotFoundException<User>(); 

        user.LockUser();

        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateRequestDto dto)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);
        
        if (user == null)
            throw new ResourceNotFoundException<User>();

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
            throw new KeyNotFoundException("User not found."); //TODO custom exception

        var hashedPassword = HashPassword(dto.Password);

        if(user.HashPassword != hashedPassword)
            throw new UnauthorizedAccessException("Invalid credentials."); //TODO custom exception
        
        return user;
    }

    public async Task AddGameToCart(Guid id, Guid gameId)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);
        if (user == null)
            throw new ResourceNotFoundException<User>(); 

        var game = await _unitOfWork.GamesRepo.GetByIdAsync(gameId);
        if (game == null)
            throw new ResourceNotFoundException<User>();

        user.AddToCart(game);

        _unitOfWork.UsersRepo.Update(user);
        await _unitOfWork.Commit();
    }

    public async Task RemoveGameFromCart(Guid id, Guid gameId)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(id);
        if (user == null)
            throw new ResourceNotFoundException<User>();

        var game = await _unitOfWork.GamesRepo.GetByIdAsync(gameId);
        if (game == null)
            throw new ResourceNotFoundException<User>();

        user.RemoveFromCart(game);
        _unitOfWork.UsersRepo.Update(user);

        await _unitOfWork.Commit();
    }

    private string GenerateRandomPassword()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
    }

    private string HashPassword(string password)
    {
        // Implement your hashing logic here
        return password; // Placeholder
    }
}