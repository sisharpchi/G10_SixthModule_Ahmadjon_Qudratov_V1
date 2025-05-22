using ContactSystem.Bll.Dtos;
using ContactSystem.Bll.Services;

namespace ContactSystem.Api.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/auth")
            .WithTags("Auth");

        userGroup.MapPost("/register",
        async (UserCreateDto user, IAuthService service) =>
        {
            return Results.Ok(await service.SignUpUserAsync(user));
        })
        .WithName("SignUp");

        userGroup.MapPost("/login",
        async (UserLoginDto user, IAuthService service, ILogger<IAuthService> logger) =>
        {
            logger.LogInformation("{UserName} login", user.UserName);
            return Results.Ok(await service.LoginUserAsync(user));
        })
        .WithName("Login");

        userGroup.MapPost("/refreshToken",
        async (RefreshRequestDto refresh, IAuthService service) =>
        {
            return Results.Ok(await service.RefreshTokenAsync(refresh));
        })
        .WithName("RefreshToken");

        userGroup.MapDelete("/logOut", 
        async (string token, IAuthService service) =>
        {
            await service.LogOutAsync(token);
        })
        .WithName("LogOut");
    }
}
