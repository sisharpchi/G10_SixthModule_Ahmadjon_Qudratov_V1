using ContactSystem.Bll.Dtos;
using ContactSystem.Bll.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContactSystem.Api.Endpoints;

public static class RoleEndpoint
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/role")
            .RequireAuthorization()
            .WithTags("Role");

        userGroup.MapGet("/getAll", [Authorize(Roles = "Admin, SuperAdmin")]
        async (IRoleService roleService) =>
        {
            var roles = await roleService.GetAllRolesAsync();
            return Results.Ok(roles);
        })
        .WithName("GetAllUsers");

        userGroup.MapPost("/post", [Authorize(Roles = "SuperAdmin")]
        async (RoleCreateDto roleCreateDto, IRoleService roleService) =>
        {
            await roleService.AddRoleAsync(roleCreateDto);
            return Results.Ok();
        })
        .WithName("AddRole");

        userGroup.MapDelete("/delete", [Authorize(Roles = "SuperAdmin")]
        async (long id, IRoleService roleService) =>
        {
            await roleService.DeleteRoleAsync(id);
            return Results.Ok();
        })
        .WithName("DeleteRole");
    }
}
