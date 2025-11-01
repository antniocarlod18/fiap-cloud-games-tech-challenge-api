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
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(dto.UserId);
        if (user == null) 
            throw new ResourceNotFoundException<User>();

        var order = new Order(user);

        await _unitOfWork.OrdersRepo.AddAsync(order);
        await _unitOfWork.Commit();

        return order;
    }

    public async Task<OrderDetailedResponseDto?> GetAsync(Guid id)
    {
        var order = await _unitOfWork.OrdersRepo.GetByIdAsync(id);
        
        if (order == null)
            throw new ResourceNotFoundException<Order>();

        return order;
    }

    public async Task<IList<OrderResponseDto>> GetByUserAsync(Guid idUser)
    {
        var user = await _unitOfWork.UsersRepo.GetByIdAsync(idUser);
        if (user == null)
            throw new ResourceNotFoundException<User>();

        var orders = await _unitOfWork.OrdersRepo.GetByUserAsync(idUser);
        if (orders == null || !orders.Any())
            return [];

        return (IList<OrderResponseDto>)orders;
    }

    public async Task CancelOrderAsync(Guid orderId)
    {
        var order = await _unitOfWork.OrdersRepo.GetByIdAsync(orderId);
        if (order == null)
            throw new ResourceNotFoundException<Order>();
        order.CancelOrder();
        _unitOfWork.OrdersRepo.Update(order);
        await _unitOfWork.Commit();
    }

    public async Task CompleteOrderAsync(Guid orderId)
    {
        var order = await _unitOfWork.OrdersRepo.GetByIdAsync(orderId);
        if (order == null)
            throw new ResourceNotFoundException<Order>();
        order.CompletedOrder();
        _unitOfWork.OrdersRepo.Update(order);
        await _unitOfWork.Commit();
    }

    public async Task RefundOrderAsync(Guid orderId)
    {
        var order = await _unitOfWork.OrdersRepo.GetByIdAsync(orderId);
        if (order == null)
            throw new ResourceNotFoundException<Order>();
        order.RefundOrder();
        _unitOfWork.OrdersRepo.Update(order);
        await _unitOfWork.Commit();
    }
}