using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using Moq;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Services;

public class PromotionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGameRepository> _gamesRepoMock;
    private readonly Mock<IPromotionRepository> _promotionsRepoMock;
    private readonly PromotionService _service;

    public PromotionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _gamesRepoMock = new Mock<IGameRepository>();
        _promotionsRepoMock = new Mock<IPromotionRepository>();

        _unitOfWorkMock.SetupGet(x => x.GamesRepo).Returns(_gamesRepoMock.Object);
        _unitOfWorkMock.SetupGet(x => x.PromotionsRepo).Returns(_promotionsRepoMock.Object);

        _service = new PromotionService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddPromotionWithValidGames()
    {
        // Arrange
        var game = new Game("Zelda", "Adventure", new List<Domain.Enums.GamePlatformEnum>(), "desc", 200, "dev", "dist", "1.0", true);
        var dto = new PromotionRequestDto
        {
            GameIds = new List<Guid> { game.Id },
            DiscountPercentage = 20,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        _gamesRepoMock.Setup(x => x.GetByIdAsync(game.Id)).ReturnsAsync(game);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.NotNull(result);
        _promotionsRepoMock.Verify(x => x.AddAsync(It.IsAny<Promotion>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(id))
                           .ReturnsAsync((Promotion?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetAsync(id));
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPromotion_WhenFound()
    {
        var id = Guid.NewGuid();
        var promo = new Promotion(new List<Game>(), 20, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1)) { Id = id };

        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(id))
                           .ReturnsAsync(promo);

        var result = await _service.GetAsync(id);

        Assert.Equal(id, result!.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoPromotions()
    {
        _promotionsRepoMock.Setup(x => x.GetAllAsync())
                           .ReturnsAsync(new List<Promotion>());

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPromotions_WhenFound()
    {
        var list = new List<Promotion>
        {
            new Promotion(new List<Game>(), 20, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
        };

        _promotionsRepoMock.Setup(x => x.GetAllAsync())
                           .ReturnsAsync(list);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnEmpty_WhenNoActivePromotions()
    {
        _promotionsRepoMock.Setup(x => x.GetActiveAsync())
                           .ReturnsAsync(new List<Promotion>());

        var result = await _service.GetActiveAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenPromotionNotFound()
    {
        var id = Guid.NewGuid();
        _promotionsRepoMock.Setup(x => x.GetByIdAsync(id))
                           .ReturnsAsync((Promotion?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteAsync(id));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenPromotionExists()
    {
        var promo = new Promotion(new List<Game>(), 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));
        _promotionsRepoMock.Setup(x => x.GetByIdAsync(promo.Id))
                           .ReturnsAsync(promo);

        await _service.DeleteAsync(promo.Id);

        _promotionsRepoMock.Verify(x => x.Delete(promo), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task AddGameToPromotionAsync_ShouldThrow_WhenPromotionNotFound()
    {
        var promoId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(promoId))
                           .ReturnsAsync((Promotion?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.AddGameToPromotionAsync(promoId, gameId));
    }

    [Fact]
    public async Task AddGameToPromotionAsync_ShouldThrow_WhenGameNotFound()
    {
        var promo = new Promotion(new List<Game>(), 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));
        var gameId = Guid.NewGuid();

        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(promo.Id))
                           .ReturnsAsync(promo);
        _gamesRepoMock.Setup(x => x.GetByIdAsync(gameId))
                      .ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.AddGameToPromotionAsync(promo.Id, gameId));
    }

    [Fact]
    public async Task AddGameToPromotionAsync_ShouldAddGameAndCommit()
    {
        var game = new Game("Halo", "FPS", new List<Domain.Enums.GamePlatformEnum>(), "desc", 150, "dev", "dist", "1.0", true);
        var promo = new Promotion(new List<Game>(), 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(promo.Id)).ReturnsAsync(promo);
        _gamesRepoMock.Setup(x => x.GetByIdAsync(game.Id)).ReturnsAsync(game);

        var result = await _service.AddGameToPromotionAsync(promo.Id, game.Id);

        Assert.NotNull(result);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        _promotionsRepoMock.Verify(x => x.Update(It.IsAny<Promotion>()), Times.Once);
    }

    [Fact]
    public async Task RemoveGameToPromotionAsync_ShouldThrow_WhenPromotionNotFound()
    {
        var promoId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(promoId))
                           .ReturnsAsync((Promotion?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.RemoveGameToPromotionAsync(promoId, gameId));
    }

    [Fact]
    public async Task RemoveGameToPromotionAsync_ShouldThrow_WhenGameNotFound()
    {
        var promo = new Promotion(new List<Game>(), 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));
        var gameId = Guid.NewGuid();

        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(promo.Id)).ReturnsAsync(promo);
        _gamesRepoMock.Setup(x => x.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.RemoveGameToPromotionAsync(promo.Id, gameId));
    }

    [Fact]
    public async Task RemoveGameToPromotionAsync_ShouldRemoveGameAndCommit()
    {
        var game = new Game("Halo", "FPS", new List<Domain.Enums.GamePlatformEnum>(), "desc", 150, "dev", "dist", "1.0", true);
        var promo = new Promotion(new List<Game> { game }, 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        _promotionsRepoMock.Setup(x => x.GetDetailedByIdAsync(promo.Id)).ReturnsAsync(promo);
        _gamesRepoMock.Setup(x => x.GetByIdAsync(game.Id)).ReturnsAsync(game);

        var result = await _service.RemoveGameToPromotionAsync(promo.Id, game.Id);

        Assert.NotNull(result);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        _promotionsRepoMock.Verify(x => x.Update(It.IsAny<Promotion>()), Times.Once);
    }
}
