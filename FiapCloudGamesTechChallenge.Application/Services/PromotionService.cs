using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;

namespace FiapCloudGamesTechChallenge.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _unitOfWork;

    public PromotionService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task<PromotionResponseDto?> AddAsync(PromotionRequestDto dto)
    {
        var games = new List<Game>();
        foreach (var gid in dto.GameIds)
        {
            var g = await _unitOfWork.GamesRepo.GetByIdAsync(gid);
            if (g != null) games.Add(g);
        }

        var promotion = new Promotion(games, dto.DiscountPercentage, dto.StartDate, dto.EndDate);
        await _unitOfWork.PromotionsRepo.AddAsync(promotion);
        await _unitOfWork.Commit();
        return promotion;
    }

    public async Task<PromotionResponseDto?> GetAsync(Guid id)
    {
        var promotion = await _unitOfWork.PromotionsRepo.GetByIdAsync(id);

        if (promotion == null)
            throw new ResourceNotFoundException<Promotion>();

        return promotion;
    }

    public async Task<IList<PromotionResponseDto>> GetActiveAsync()
    {
        var promotions = await _unitOfWork.PromotionsRepo.GetActiveAsync();

        if (promotions == null || !promotions.Any())
            return [];

        return (IList<PromotionResponseDto>)promotions;
    }

    public async Task<PromotionResponseDto?> DeleteAsync(Guid id)
    {
        var promotion = await _unitOfWork.PromotionsRepo.GetByIdAsync(id);

        if (promotion == null)
            throw new ResourceNotFoundException<Promotion>();

        _unitOfWork.PromotionsRepo.Delete(promotion);

        return promotion;
    }

    public async Task<PromotionResponseDto?> AddGameToPromotionAsync(Guid id, Guid gameId)
    {
        var promotion = await _unitOfWork.PromotionsRepo.GetByIdAsync(id);

        if (promotion == null)
            throw new ResourceNotFoundException<Promotion>();

        var game = await _unitOfWork.GamesRepo.GetByIdAsync(gameId);

        if (game == null)
            throw new ResourceNotFoundException<Promotion>();

        promotion.AddGame(game);

        _unitOfWork.PromotionsRepo.Update(promotion);

        return promotion;
    }

    public async Task<PromotionResponseDto?> RemoveGameToPromotionAsync(Guid id, Guid gameId)
    {
        var promotion = await _unitOfWork.PromotionsRepo.GetByIdAsync(id);

        if (promotion == null)
            throw new ResourceNotFoundException<Promotion>();

        var game = await _unitOfWork.GamesRepo.GetByIdAsync(gameId);

        if (game == null)
            throw new ResourceNotFoundException<Promotion>();

        promotion.RemoveGame(game);

        _unitOfWork.PromotionsRepo.Update(promotion);

        return promotion;
    }
}