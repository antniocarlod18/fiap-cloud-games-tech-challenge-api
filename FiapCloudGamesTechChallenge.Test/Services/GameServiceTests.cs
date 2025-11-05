using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using Moq;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Services;

public class GameServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGameRepository> _gameRepoMock;
    private readonly Mock<IAuditGamePriceRepository> _auditRepoMock;
    private readonly GameService _service;

    public GameServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _gameRepoMock = new Mock<IGameRepository>();
        _auditRepoMock = new Mock<IAuditGamePriceRepository>();

        _unitOfWorkMock.Setup(u => u.GamesRepo).Returns(_gameRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.AuditGamePriceRepo).Returns(_auditRepoMock.Object);

        _service = new GameService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddGame_WhenValidRequest()
    {
        // Arrange
        var dto = new GameRequestDto
        {
            Title = "Halo Infinite",
            Genre = "Shooter",
            Description = "Sci-fi FPS",
            Price = 299.90m,
            Developer = "343 Industries",
            Distributor = "Microsoft",
            GamePlatforms = new List<string> { "XboxSeriesX", "PC" },
            GameVersion = "1.0",
            Available = true
        };

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        _gameRepoMock.Verify(g => g.AddAsync(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        Assert.Equal(dto.Title, result!.Title);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnGame_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var game = new Game("FIFA 25", "Sports", [GamePlatformEnum.PC], "Soccer", 350, "EA", "EA", "1.0", true);

        _gameRepoMock.Setup(g => g.GetWithPromotionsByIdAsync(id)).ReturnsAsync(game);

        // Act
        var result = await _service.GetAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FIFA 25", result!.Title);
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenGameNotFound()
    {
        // Arrange
        _gameRepoMock.Setup(g => g.GetWithPromotionsByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndAudit_WhenPriceChanges()
    {
        // Arrange
        var id = Guid.NewGuid();
        var game = new Game("Zelda", "Adventure", [GamePlatformEnum.NintendoSwitch], "Open world", 200, "Nintendo", "Nintendo", "1.0", true);

        _gameRepoMock.Setup(g => g.GetWithPromotionsByIdAsync(id)).ReturnsAsync(game);

        var dto = new GameRequestDto
        {
            Title = "Zelda Updated",
            Genre = "Adventure",
            Description = "Updated description",
            Price = 250,
            Developer = "Nintendo",
            Distributor = "Nintendo",
            GamePlatforms = new List<string> { "NintendoSwitch" },
            GameVersion = "1.1",
            Available = true
        };

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        _auditRepoMock.Verify(a => a.AddAsync(It.IsAny<AuditGamePrice>()), Times.Once);
        _gameRepoMock.Verify(g => g.Update(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        Assert.Equal(dto.Title, result!.Title);
        Assert.Equal(250, result.Price);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenGameNotFound()
    {
        _gameRepoMock.Setup(g => g.GetWithPromotionsByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        var dto = new GameRequestDto { Title = "Ghost Game", GamePlatforms = ["PC"] };

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.UpdateAsync(Guid.NewGuid(), dto));
    }

    [Fact]
    public async Task GetAllAvailableAsync_ShouldReturnEmptyList_WhenNoGames()
    {
        _gameRepoMock.Setup(g => g.GetAvailableAsync()).ReturnsAsync(new List<Game>());

        var result = await _service.GetAllAvailableAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTitleAsync_ShouldReturnGame_WhenFound()
    {
        var game = new Game("GTA VI", "Action", [GamePlatformEnum.PC], "Open world", 499, "Rockstar", "Take-Two", "1.0", true);

        _gameRepoMock.Setup(g => g.GetByTitleAsync("GTA VI")).ReturnsAsync(game);

        var result = await _service.GetByTitleAsync("GTA VI");

        Assert.NotNull(result);
        Assert.Equal("GTA VI", result.Title);
    }

    [Fact]
    public async Task GetByTitleAsync_ShouldThrow_WhenNotFound()
    {
        _gameRepoMock.Setup(g => g.GetByTitleAsync(It.IsAny<string>())).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByTitleAsync("MissingGame"));
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetAvailableFalse_WhenExists()
    {
        var id = Guid.NewGuid();
        var game = new Game("COD", "Shooter", [GamePlatformEnum.PC], "War game", 300, "Activision", "Blizzard", "1.0", true);

        _gameRepoMock.Setup(g => g.GetByIdAsync(id)).ReturnsAsync(game);

        await _service.DeleteAsync(id);

        Assert.False(game.Available);
        _gameRepoMock.Verify(g => g.Update(game), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _gameRepoMock.Setup(g => g.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }
}
