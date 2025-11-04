using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;

namespace FiapCloudGamesTechChallenge.Application.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task<IList<AuditGameUserCollectionResponseDto?>> GetByUserAsync(Guid userId, string? collection)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(userId);
        
        if(user == null)
            throw new ResourceNotFoundException(nameof(User));

        var listOfAudits = await _unitOfWork.AuditGameUsersRepo.GetByUserAsync(userId, Convert(collection));

        if (listOfAudits == null || !listOfAudits.Any()) return [];

        return listOfAudits.Select(x => (AuditGameUserCollectionResponseDto?)x).ToList();
    }

    public async Task<IList<AuditGameUserCollectionResponseDto?>> GetByGameAsync(Guid gameId, string? collection)
    {
        var game = await _unitOfWork.GamesRepo.GetByIdAsync(gameId); 

        if (game == null)
            throw new ResourceNotFoundException(nameof(Game));

        var listOfAudits = await _unitOfWork.AuditGameUsersRepo.GetByGameAsync(gameId, Convert(collection));

        if (listOfAudits == null || !listOfAudits.Any()) return [];

        return listOfAudits.Select(x => (AuditGameUserCollectionResponseDto?)x).ToList();
    }

    public async Task<IList<AuditGamePriceResponseDto?>> GetByGameAsync(Guid gameId)
    {
        var game = await _unitOfWork.GamesRepo.GetByIdAsync(gameId);

        if (game == null)
            throw new ResourceNotFoundException(nameof(Game));

        var listOfAudits = await _unitOfWork.AuditGamePriceRepo.GetByGameAsync(gameId);

        if (listOfAudits == null || !listOfAudits.Any()) return [];

        return listOfAudits.Select(x => (AuditGamePriceResponseDto?)x).ToList();
    }

    private AuditGameUserCollectionEnum? Convert(string? collection)
    {
        if (collection == null)
            return null;

        return (AuditGameUserCollectionEnum)Enum.Parse(typeof(AuditGameUserCollectionEnum), collection);
    }
}