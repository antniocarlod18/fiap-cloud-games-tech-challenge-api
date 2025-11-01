using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;

namespace FiapCloudGamesTechChallenge.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/orders", AddAsync)
            .RequireAuthorization("SameUserOrAdmin");

        endpoints.MapGet("/orders/{id}", GetAsync)
            .RequireAuthorization("SameUserOrAdmin"); 

        endpoints.MapGet("/users/{idUser}/orders", GetByUserAsync)
            .RequireAuthorization("SameUserOrAdmin");

        return endpoints;
    }

    public static async Task<IResult> AddAsync(OrderRequestDto dto, IOrderService service)
    {
        var order = await service.AddAsync(dto);
        return Results.Created($"/orders/{order.Id}", order);
    }

    public static async Task<IResult> GetAsync(Guid id, IOrderService service)
    {
        return Results.Ok(await service.GetAsync(id));
    }

    public static async Task<IResult> GetByUserAsync(Guid idUser, IOrderService service)
    {
        return Results.Ok(await service.GetByUserAsync(idUser));
    }
}