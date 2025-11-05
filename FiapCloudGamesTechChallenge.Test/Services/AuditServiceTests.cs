using Xunit;
using Moq;
using FiapCloudGamesTechChallenge.Application.Services;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Application.Dtos;

namespace FiapCloudGamesTechChallenge.Test.Services;

public class AuditServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AuditService _service;

    public AuditServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new AuditService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByUserAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.UsersRepo.GetByIdAsync(userId))
                       .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByUserAsync(userId, null));
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnEmptyList_WhenNoAuditsFound()
    {
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.UsersRepo.GetByIdAsync(userId))
                       .ReturnsAsync(new User("John", "hashed123", "john@test.com"));

        _unitOfWorkMock.Setup(u => u.AuditGameUsersRepo.GetByUserAsync(userId, null))
                       .ReturnsAsync(new List<AuditGameUserCollection>());

        // Act
        var result = await _service.GetByUserAsync(userId, null);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnMappedDtos_WhenAuditsExist()
    {
        var userId = Guid.NewGuid();
        var user = new User("John", "hashed123", "john@test.com");
        var game = new Game("Game1", "Action", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "desc", 100, "dev", "dist", "1.0", true);
        var audits = new List<AuditGameUserCollection>
        {
            new AuditGameUserCollection(user, game, AuditGameUserActionEnum.Added, AuditGameUserCollectionEnum.Cart, "Note 1")
        };

        _unitOfWorkMock.Setup(u => u.UsersRepo.GetByIdAsync(userId)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.AuditGameUsersRepo.GetByUserAsync(userId, null)).ReturnsAsync(audits);

        // Act
        var result = await _service.GetByUserAsync(userId, null);

        // Assert
        Assert.Single(result);
        Assert.IsType<AuditGameUserCollectionResponseDto>(result.First());
    }

    [Fact]
    public async Task GetByGameAsync_ShouldThrow_WhenGameNotFound()
    {
        var gameId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.GamesRepo.GetByIdAsync(gameId))
                       .ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByGameAsync(gameId, null));
    }

    [Fact]
    public async Task GetByGameAsync_ShouldReturnEmptyList_WhenNoAuditsFound()
    {
        var gameId = Guid.NewGuid();
        var game = new Game("Game1", "Action", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "desc", 100, "dev", "dist", "1.0", true);

        _unitOfWorkMock.Setup(u => u.GamesRepo.GetByIdAsync(gameId)).ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.AuditGameUsersRepo.GetByGameAsync(gameId, null))
                       .ReturnsAsync(new List<AuditGameUserCollection>());

        var result = await _service.GetByGameAsync(gameId, null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByGameAsync_ShouldReturnMappedDtos_WhenAuditsExist()
    {
        var gameId = Guid.NewGuid();
        var user = new User("John", "hashed123", "john@test.com");
        var game = new Game("Game1", "Action", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "desc", 100, "dev", "dist", "1.0", true);
        var audits = new List<AuditGameUserCollection>
        {
            new AuditGameUserCollection(user, game, AuditGameUserActionEnum.Removed, AuditGameUserCollectionEnum.Library, "Note 1")
        };

        _unitOfWorkMock.Setup(u => u.GamesRepo.GetByIdAsync(gameId)).ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.AuditGameUsersRepo.GetByGameAsync(gameId, null)).ReturnsAsync(audits);

        var result = await _service.GetByGameAsync(gameId, null);

        Assert.Single(result);
        Assert.IsType<AuditGameUserCollectionResponseDto>(result.First());
    }

    [Fact]
    public async Task GetByGamePriceAsync_ShouldThrow_WhenGameNotFound()
    {
        var gameId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.GamesRepo.GetByIdAsync(gameId))
                       .ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByGameAsync(gameId));
    }

    [Fact]
    public async Task GetByGamePriceAsync_ShouldReturnMappedDtos_WhenAuditsExist()
    {
        var gameId = Guid.NewGuid();
        var game = new Game("Game1", "Action", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "desc", 100, "dev", "dist", "1.0", true);
        var audits = new List<AuditGamePrice>
        {
            new AuditGamePrice(game, 100, 80, "Discount applied")
        };

        _unitOfWorkMock.Setup(u => u.GamesRepo.GetByIdAsync(gameId)).ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.AuditGamePriceRepo.GetByGameAsync(gameId)).ReturnsAsync(audits);

        var result = await _service.GetByGameAsync(gameId);

        Assert.Single(result);
        Assert.IsType<AuditGamePriceResponseDto>(result.First());
    }
}
