using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;

namespace FiapCloudGamesTechChallenge.Application.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;

    public GameService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task<GameResponseDto?> AddAsync(GameRequestDto gameRequestDto)
    {
        var platforms = gameRequestDto.GamePlatforms
            .Select(s => (GamePlatformEnum)Enum.Parse(typeof(GamePlatformEnum), s))
            .ToList();

        var game = new Game(
            gameRequestDto.Title,
            gameRequestDto.Genre,
            platforms,
            gameRequestDto.Description,
            gameRequestDto.Price,
            gameRequestDto.Developer,
            gameRequestDto.Distributor,
            gameRequestDto.GameVersion,
            gameRequestDto.Available
        );

        await _unitOfWork.GamesRepo.AddAsync(game);
        await _unitOfWork.Commit();

        return game;
    }

    public async Task<GameResponseDto?> GetAsync(Guid id)
    {
        var game = await _unitOfWork.GamesRepo.GetByIdAsync(id);

        if(game == null)
            throw new ResourceNotFoundException<Game>();

        return game;
    }

    public async Task<GameResponseDto?> UpdateAsync(Guid id, GameUpdateRequestDto gameRequestDto)
    {
        var game = await _unitOfWork.GamesRepo.GetByIdAsync(id);

        if (game == null)
            throw new ResourceNotFoundException<Game>();

        var platforms = gameRequestDto.GamePlatforms
            .Select(s => (GamePlatformEnum)Enum.Parse(typeof(GamePlatformEnum), s))
            .ToList();

        game.Title = gameRequestDto.Title;
        game.Genre = gameRequestDto.Genre;
        game.Description = gameRequestDto.Description;
        game.Developer = gameRequestDto.Developer;
        game.Distributor = gameRequestDto.Distributor;
        game.GamePlatforms = platforms;
        game.GameVersion = gameRequestDto.GameVersion;
        game.Available = gameRequestDto.Available;
        game.DateUpdated = DateTime.UtcNow;

        _unitOfWork.GamesRepo.Update(game);
        await _unitOfWork.Commit();
        return game;
    }

    public async Task<IList<GameResponseDto?>> GetAllAvailableAsync()
    {
        var listOfAvailableGames = await _unitOfWork.GamesRepo.GetAvailableAsync();

        if (listOfAvailableGames == null || !listOfAvailableGames.Any())
            return [];

        return listOfAvailableGames.Select(x => (GameResponseDto?)x).ToList(); 
    }

    public async Task<GameResponseDto> GetByTitleAsync(string title)
    {
        var game = await _unitOfWork.GamesRepo.GetByTitleAsync(title);

        if (game == null)
            throw new ResourceNotFoundException<Game>();

        return game;
    }

    public async Task<IList<GameResponseDto?>> GetByGenreAsync(string genre)
    {
        var listOfGames = await _unitOfWork.GamesRepo.GetByGenreAsync(genre);

        if (listOfGames == null || !listOfGames.Any())
            return [];

        return listOfGames.Select(x => (GameResponseDto?)x).ToList();
    }
}
