using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using Moq;
using Xunit;

namespace FiapCloudGamesTechChallenge.Test.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOrderRepository> _ordersRepoMock;
    private readonly Mock<IUserRepository> _usersRepoMock;
    private readonly Mock<IAuditGameUserCollectionRepository> _auditRepoMock;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _ordersRepoMock = new Mock<IOrderRepository>();
        _usersRepoMock = new Mock<IUserRepository>();
        _auditRepoMock = new Mock<IAuditGameUserCollectionRepository>();

        _unitOfWorkMock.SetupGet(x => x.OrdersRepo).Returns(_ordersRepoMock.Object);
        _unitOfWorkMock.SetupGet(x => x.UsersRepo).Returns(_usersRepoMock.Object);
        _unitOfWorkMock.SetupGet(x => x.AuditGameUsersRepo).Returns(_auditRepoMock.Object);

        _service = new OrderService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var dto = new OrderRequestDto { UserId = Guid.NewGuid() };
        _usersRepoMock.Setup(x => x.GetDetailedByIdAsync(dto.UserId))
                      .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.AddAsync(dto));
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenUserHasPendingOrder()
    {
        var user = new User("John Doe", "123", "john@doe.com")
        {
            Orders = new List<Order> { new Order() { Status = OrderStatusEnum.WaitingForPayment } }
        };

        var dto = new OrderRequestDto { UserId = user.Id };

        _usersRepoMock.Setup(x => x.GetDetailedByIdAsync(dto.UserId))
                      .ReturnsAsync(user);

        await Assert.ThrowsAsync<UserAlreadyHasAnOpenOrderException>(() => _service.AddAsync(dto));
    }

    [Fact]
    public async Task AddAsync_ShouldCreateOrder_WhenValid()
    {
        var user = new User("John Doe", "123", "john@doe.com") { 
            GameCart = new List<Game>() { 
                new Game("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 20m, "343", "MS", "1.0", true) 
            } 
        };
        var order = new Order();
        var dto = new OrderRequestDto { UserId = user.Id };

        _usersRepoMock.Setup(x => x.GetDetailedByIdAsync(user.Id))
                      .ReturnsAsync(user);

        _ordersRepoMock.Setup(x => x.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.NotNull(result);
        _ordersRepoMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenOrderNotFound()
    {
        var id = Guid.NewGuid();
        _ordersRepoMock.Setup(x => x.GetDetailedByIdAsync(id))
                       .ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetAsync(id));
    }

    [Fact]
    public async Task GetAsync_ShouldReturnOrder_WhenFound()
    {
        var user = new User("John Doe", "123", "john@doe.com")
        {
            GameCart = new List<Game>() {
                new Game("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 20m, "343", "MS", "1.0", true)
            }
        };
        var order = new Order(user);

        _ordersRepoMock.Setup(x => x.GetDetailedByIdAsync(order.Id))
            .Returns(Task.FromResult<Order?>(order));                           

        var result = await _service.GetAsync(order.Id);

        Assert.Equal(order.Id, result!.Id);
    }

    [Fact]
    public async Task GetByUserAsync_ShouldThrow_WhenUserNotFound()
    {
        var id = Guid.NewGuid();
        _usersRepoMock.Setup(x => x.GetByIdAsync(id))
                      .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.GetByUserAsync(id));
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnEmpty_WhenNoOrders()
    {
        var id = Guid.NewGuid();
        _usersRepoMock.Setup(x => x.GetByIdAsync(id))
                      .ReturnsAsync(new User("a", "b", "c"));
        _ordersRepoMock.Setup(x => x.GetByUserAsync(id))
                       .ReturnsAsync(new List<Order>());

        var result = await _service.GetByUserAsync(id);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldThrow_WhenOrderNotFoundOrUnauthorized()
    {
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _ordersRepoMock.Setup(x => x.GetDetailedByIdAsync(orderId))
                       .ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.CancelOrderAsync(orderId, userId));
    }

    [Fact]
    public async Task CompleteOrderAsync_ShouldUpdateAndCommit()
    {
        var user = new User("Admin", "123", "a@b.com") { IsAdmin = true,
            GameCart = new List<Game>() {
                new Game("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 20m, "343", "MS", "1.0", true)
            }
        };
        var game = new Game("Title", "Genre", new List<GamePlatformEnum>(), "desc", 10, "dev", "dist", "1.0", true);
        var order = new Order(user);
        order.Games.Add(new OrderGameItem(order, game));
        var dto = new OrderDetailedResponseDto { Id = order.Id, User = user };

        _ordersRepoMock.Setup(x => x.GetDetailedByIdAsync(order.Id)).ReturnsAsync(order);

        var result = await _service.CompleteOrderAsync(order.Id, user.Id);

        Assert.NotNull(result);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task RefundOrderAsync_ShouldUpdateAndCommit()
    {
        var user = new User("Admin", "123", "a@b.com") { IsAdmin = true,
            GameCart = new List<Game>() {
                new Game("Halo", "FPS", new List<GamePlatformEnum> { GamePlatformEnum.PC }, "Desc", 20m, "343", "MS", "1.0", true)
            }
        };
        var game = new Game("Title", "Genre", new List<GamePlatformEnum>(), "desc", 10, "dev", "dist", "1.0", true);
        var order = new Order(user) { Status= OrderStatusEnum.Completed };
        order.Games.Add(new OrderGameItem(order, game));
        var dto = new OrderDetailedResponseDto { Id = order.Id, User = user };

        _ordersRepoMock.Setup(x => x.GetDetailedByIdAsync(order.Id)).ReturnsAsync(order);

        var result = await _service.RefundOrderAsync(order.Id, user.Id);

        Assert.NotNull(result);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }
}
