using ContactSystem.Bll.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContactSystem.Api.Endpoints;

public static class AdminEndpoint
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/admin")
            .RequireAuthorization()
            .WithTags("Admin");

        userGroup.MapDelete("/delete", [Authorize(Roles = "Admin, SuperAdmin")]
        async (long userId, HttpContext httpContext, IUserService userService, ILogger<IUserService> logger) =>
        {
            var role = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            logger.LogInformation("{User} deleted: ", userId);
            await userService.DeleteUserByIdAsync(userId, role);
            return Results.Ok();
        })
        .WithName("DeleteUser");

        userGroup.MapPatch("/changeRole", [Authorize(Roles = "SuperAdmin")]
        async (long userId, string userRole, IUserService userService, ILogger<IUserService> logger) =>
        {
            logger.LogInformation("{User} change role", userId);
            await userService.UpdateUserRoleAsync(userId, userRole);
            return Results.Ok();
        })
        .WithName("UpdateUserRole");
        
        userGroup.MapGet("/getAllUsersByRole", [Authorize(Roles = "Admin, SuperAdmin")][ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any, NoStore = false)]
        async (string role, IRoleService roleService) =>
        {
            var users = await roleService.GetAllUsersByRoleAsync(role);
            return Results.Ok(users);
        })
        .WithName("GetUsersByRole")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
