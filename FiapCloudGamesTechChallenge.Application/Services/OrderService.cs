using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using FiapCloudGamesTechChallenge.Domain.Repositories;

namespace FiapCloudGamesTechChallenge.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    public async Task<OrderResponseDto?> AddAsync(OrderRequestDto dto)
    {
        var user = await _unitOfWork.UsersRepo.GetDetailedByIdAsync(dto.UserId);
        if (user == null) 
            throw new ResourceNotFoundException(nameof(User));

        if(user.Orders.Any(o => o.Status == Domain.Enums.OrderStatusEnum.WaitingForPayment))
            throw new UserAlreadyHasAnOpenOrderException();

        var order = new Order(user);

        await _unitOfWork.OrdersRepo.AddAsync(order);
        foreach (var game in order.Games)
        {
            await _unitOfWork.AuditGameUsersRepo.AddAsync(
                new AuditGameUserCollection(
                    order.User,
                    game.Game,
                    Domain.Enums.AuditGameUserActionEnum.Added,
                    Domain.Enums.AuditGameUserCollectionEnum.Cart,
                    "Game removed from cart to open an order."));
        }
        await _unitOfWork.Commit();

        return order;
    }

    public async Task<OrderDetailedResponseDto?> GetAsync(Guid id)
    {
        var order = await _unitOfWork.OrdersRepo.GetDetailedByIdAsync(id);
        
        if (order == null)
            throw new ResourceNotFoundException(nameof(Order));

        return order;
    }

    public async Task<IList<OrderResponseDto?>> GetByUserAsync(Guid idUser)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(idUser);
        if (user == null)
            throw new ResourceNotFoundException(nameof(User));

        var orders = await _unitOfWork.OrdersRepo.GetByUserAsync(idUser);
        if (orders == null || !orders.Any())
            return [];

        return orders.Select(x => (OrderResponseDto?)x).ToList();
    }

    public async Task<OrderDetailedResponseDto?> CancelOrderAsync(Guid orderId, Guid idUser)
    {
        var order = await _unitOfWork.OrdersRepo.GetDetailedByIdAsync(orderId);
        if (order == null || (order.User.Id != idUser && !order.User.IsAdmin))
            throw new ResourceNotFoundException(nameof(Order));
        
        order.CancelOrder();
        
        _unitOfWork.OrdersRepo.Update(order);
        foreach (var game in order.Games)
        {
            await _unitOfWork.AuditGameUsersRepo.AddAsync(
                new AuditGameUserCollection(
                    order.User,
                    game.Game,
                    Domain.Enums.AuditGameUserActionEnum.Added,
                    Domain.Enums.AuditGameUserCollectionEnum.Cart,
                    "Game returned to user cart due to order cancellation"));
        }
        await _unitOfWork.Commit();
        return order;
    }

    public async Task<OrderDetailedResponseDto?> CompleteOrderAsync(Guid orderId, Guid idUser)
    {
        var order = await _unitOfWork.OrdersRepo.GetDetailedByIdAsync(orderId);
        if (order == null || (order.User.Id != idUser && !order.User.IsAdmin))
            throw new ResourceNotFoundException(nameof(Order));

        order.CompletedOrder();

        _unitOfWork.OrdersRepo.Update(order);
        foreach (var game in order.Games)
        {
            await _unitOfWork.AuditGameUsersRepo.AddAsync(
                new AuditGameUserCollection(
                    order.User,
                    game.Game,
                    Domain.Enums.AuditGameUserActionEnum.Added,
                    Domain.Enums.AuditGameUserCollectionEnum.Library,
                    "Game added to library on completing order."));
        }

        await _unitOfWork.Commit();
        return order;
    }

    public async Task<OrderDetailedResponseDto?> RefundOrderAsync(Guid orderId, Guid idUser)
    {
        var order = await _unitOfWork.OrdersRepo.GetDetailedByIdAsync(orderId);
        if (order == null || (order.User.Id != idUser && !order.User.IsAdmin))
            throw new ResourceNotFoundException(nameof(Order));

        order.RefundOrder();

        _unitOfWork.OrdersRepo.Update(order);
        foreach (var game in order.Games)
        {
            await _unitOfWork.AuditGameUsersRepo.AddAsync(
                new AuditGameUserCollection(
                    order.User,
                    game.Game,
                    Domain.Enums.AuditGameUserActionEnum.Removed,
                    Domain.Enums.AuditGameUserCollectionEnum.Library,
                    "Game removed from library due to order refunding."));
        }
        await _unitOfWork.Commit();
        return order;
    }
}