using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;

namespace FiapCloudGamesTechChallenge.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/authentication", AuthAsync);

        return endpoints;
    }

    public static async Task<IResult> AuthAsync(AuthRequestDto authRequestDto, IAuthService service)
    {
        var token = await service.GenerateJwtTokenAsync(authRequestDto);
        return Results.Ok(token);
    }
}
