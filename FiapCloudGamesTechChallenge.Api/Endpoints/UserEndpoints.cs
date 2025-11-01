
using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;

namespace FiapCloudGamesTechChallenge.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/users", AddAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapGet("/users/{userId}", Getsync)
            .RequireAuthorization("SameUserOrAdmin"); 

        endpoints.MapPut("/users/{userId}/unlock", UnlockAsync)
            .RequireAuthorization("SameUserOrAdmin");

        endpoints.MapPut("/users/{userId}/make-admin", MakeAdminAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapPut("/users/{userId}/revoke-admin", RevokeAdminAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapGet("/users", GetAllAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapDelete("/users/{userId}", DeleteAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapPut("/users/{userId}", UpdateAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapPost("/users/{userId}/games/{gameId}/cart", AddGameToCart)
            .RequireAuthorization("SameUserOrAdmin"); 

        endpoints.MapDelete("/users/{userId}/games/{gameId}/cart", RemoveGameFromCart)
            .RequireAuthorization("SameUserOrAdmin");

        return endpoints;
    }

    public static async Task<IResult> AddAsync(UserRequestDto dto, IUserService service)
    {
        var created = await service.AddAsync(dto);
        return Results.Created("/users", created);
    }

    public static async Task<IResult> Getsync(Guid userId, IUserService service)
    {
        return Results.Ok(await service.GetAsync(userId));
    }

    public static async Task<IResult> UnlockAsync(Guid userId, UserUnlockRequestDto userUnlockRequestDto, IUserService service)
    {
        return Results.Ok(await service.UnlockAsync(userId, userUnlockRequestDto));
    }

    public static async Task<IResult> MakeAdminAsync(Guid userId, IUserService service)
    {
        return Results.Ok(await service.MakeAdminAsync(userId));
    }

    public static async Task<IResult> RevokeAdminAsync(Guid userId, IUserService service)
    {
        return Results.Ok(await service.RevokeAdminAsync(userId));
    }

    public static async Task<IResult> GetAllAsync( IUserService service)
    {
        return Results.Ok(await service.GetAllAsync());
    }

    public static async Task<IResult> DeleteAsync(Guid userId, IUserService service)
    {
        await service.DeleteAsync(userId);
        return Results.Ok();
    }

    public static async Task<IResult> UpdateAsync(Guid userId, UserUpdateRequestDto dto, IUserService service)
    {
        return Results.Ok(await service.UpdateAsync(userId, dto));
    }

    public static async Task<IResult> AddGameToCart(Guid userId, Guid gameId, IUserService service)
    {
        await service.AddGameToCart(userId, gameId);
        return Results.Ok();
    }

    public static async Task<IResult> RemoveGameFromCart(Guid userId, Guid gameId, IUserService service)
    {
        await service.RemoveGameFromCart(userId, gameId);
        return Results.Ok();
    }
}