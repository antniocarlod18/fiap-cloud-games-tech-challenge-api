using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGamesTechChallenge.Api.Endpoints;

public static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/games", AddAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin")); ;

        endpoints.MapGet("/games/{id}", GetAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User")); ;

        endpoints.MapGet("/games/title", GetByTitleAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User")); ;

        endpoints.MapGet("/games/genre", GetByGenreAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User")); ;

        endpoints.MapGet("/games/available", GetAllAvailableAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "User")); ;

        endpoints.MapPut("/games/{id}", UpdateAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin")); ;

        return endpoints;
    }

    public static async Task<IResult> AddAsync(GameRequestDto gameRequestDto, IGameService gameService) 
    {
        return Results.Created("/games", await gameService.AddAsync(gameRequestDto));
    }

    public static async Task<IResult> GetAsync(Guid id, IGameService gameService)
    {
        return Results.Ok(await gameService.GetAsync(id));
    }

    public static async Task<IResult> GetByTitleAsync([FromQuery]string title, IGameService gameService)
    {
        return Results.Ok(await gameService.GetByTitleAsync(title));
    }

    public static async Task<IResult> GetByGenreAsync([FromQuery]string genre, IGameService gameService)
    {
        return Results.Ok(await gameService.GetByGenreAsync(genre));
    }

    public static async Task<IResult> GetAllAvailableAsync(IGameService gameService)
    {
        return Results.Ok(await gameService.GetAllAvailableAsync());
    }

    public static async Task<IResult> UpdateAsync(Guid id, GameUpdateRequestDto gameRequestDto, IGameService gameService)
    {
        return Results.Ok(await gameService.UpdateAsync(id, gameRequestDto));
    }
}
