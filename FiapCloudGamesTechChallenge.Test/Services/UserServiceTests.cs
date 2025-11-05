using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IGameRepository> _gameRepoMock;
    private readonly Mock<IAuditGameUserCollectionRepository> _auditRepoMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _gameRepoMock = new Mock<IGameRepository>();
        _auditRepoMock = new Mock<IAuditGameUserCollectionRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();

        _unitOfWorkMock.SetupGet(u => u.UsersRepo).Returns(_userRepoMock.Object);
        _unitOfWorkMock.SetupGet(u => u.GamesRepo).Returns(_gameRepoMock.Object);
        _unitOfWorkMock.SetupGet(u => u.AuditGameUsersRepo).Returns(_auditRepoMock.Object);

        _service = new UserService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task AddAsync_Should_Add_User_And_Commit()
    {
        // Arrange
        var dto = new UserRequestDto { Name = "John", Email = "john@test.com" };

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        Assert.Equal(dto.Name, result!.Name);
        Assert.Equal(dto.Email, result!.Email);
    }

    [Fact]
    public async Task GetAsync_Should_Return_User_When_Found()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User("Test", "hashed", "test@test.com");
        _userRepoMock.Setup(r => r.GetDetailedByIdAsync(id)).ReturnsAsync(user);

        // Act
        var result = await _service.GetAsync(id);

        // Assert
        Assert.Equal(user.Name, result!.Name);
    }

    [Fact]
    public async Task GetAsync_Should_Throw_When_Not_Found()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetDetailedByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UnlockAsync_Should_Update_User_And_Commit()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User("John", "hash", "john@test.com");
        var dto = new UserUnlockRequestDto { Password = "1234" };
        _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

        // Act
        var result = await _service.UnlockAsync(id, dto);

        // Assert
        _userRepoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        Assert.Equal(user.Email, result!.Email);
    }

    [Fact]
    public async Task UnlockAsync_Should_Throw_When_User_Not_Found()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            _service.UnlockAsync(Guid.NewGuid(), new UserUnlockRequestDto { Password = "1234" }));
    }

    [Fact]
    public async Task MakeAdminAsync_Should_Update_And_Commit()
    {
        var id = Guid.NewGuid();
        var user = new User("Admin", "pwd", "a@a.com");
        _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

        var result = await _service.MakeAdminAsync(id);

        _userRepoMock.Verify(r => r.Update(It.Is<User>(x => x.IsAdmin)), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        Assert.True(result!.IsAdmin);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Throw_When_User_Not_Found()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        await Assert.ThrowsAsync<AuthorizationException>(() =>
            _service.AuthenticateAsync(new UserAuthenticateRequestDto { Email = "x", Password = "x" }));
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Throw_When_Password_Wrong()
    {
        var user = new User("u", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("pass")), "u@test.com");
        _userRepoMock.Setup(r => r.GetByEmailAsync("u@test.com")).ReturnsAsync(user);

        await Assert.ThrowsAsync<AuthorizationException>(() =>
            _service.AuthenticateAsync(new UserAuthenticateRequestDto { Email = "u@test.com", Password = "wrong" }));
    }

    [Fact]
    public async Task AddGameToCart_Should_Add_And_Commit()
    {
        var user = new User("User", "pwd", "user@test.com");
        var game = new Game("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 20m, "343", "MS", "1.0", true);
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        _userRepoMock.Setup(r => r.GetDetailedByIdAsync(userId)).ReturnsAsync(user);
        _gameRepoMock.Setup(r => r.GetWithPromotionsByIdAsync(gameId)).ReturnsAsync(game);

        await _service.AddGameToCart(userId, gameId);

        _userRepoMock.Verify(r => r.Update(user), Times.Once);
        _auditRepoMock.Verify(a => a.AddAsync(It.IsAny<AuditGameUserCollection>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task AddGameToCart_Should_Throw_When_User_Not_Found()
    {
        _userRepoMock.Setup(r => r.GetDetailedByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.AddGameToCart(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task RemoveGameFromCart_Should_Throw_When_Game_Not_Found()
    {
        var user = new User("User", "pwd", "user@test.com");
        _userRepoMock.Setup(r => r.GetDetailedByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _gameRepoMock.Setup(r => r.GetWithPromotionsByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.RemoveGameFromCart(Guid.NewGuid(), Guid.NewGuid()));
    }
}
