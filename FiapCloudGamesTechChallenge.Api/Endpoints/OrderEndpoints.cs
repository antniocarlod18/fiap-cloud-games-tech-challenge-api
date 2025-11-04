using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FluentValidation;
using System.Security.Claims;

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

        endpoints.MapPost("/orders/{id}/cancel", CancelAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

        endpoints.MapPost("/orders/{id}/complete", CompleteAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

        endpoints.MapPost("/orders/{id}/refund", RefundAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

        return endpoints;
    }

    public static async Task<IResult> AddAsync(OrderRequestDto dto, IOrderService service, IValidator<OrderRequestDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

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

    public static async Task<IResult> CancelAsync(Guid id, IOrderService service, HttpContext context)
    {
        Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
        var order = await service.CancelOrderAsync(id, userId);
        return Results.Created($"/orders/{order.Id}", order);
    }

    public static async Task<IResult> CompleteAsync(Guid id, IOrderService service, HttpContext context)
    {
        Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
        var order = await service.CompleteOrderAsync(id, userId);
        return Results.Created($"/orders/{order.Id}", order);
    }

    public static async Task<IResult> RefundAsync(Guid id, IOrderService service, HttpContext context)
    {
        Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
        var order = await service.RefundOrderAsync(id, userId);
        return Results.Created($"/orders/{order.Id}", order);
    }
}