using ContactSystem.Bll.Dtos;
using ContactSystem.Bll.Services;
using ContactSystem.Core.Errors;
using Microsoft.AspNetCore.Authorization;

namespace ContactSystem.Api.Endpoints;

public static class ContactEndpoint
{
    public static void MapContactEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/contact")
            .RequireAuthorization()
            .WithTags("Contact");

        userGroup.MapPost("/post", [Authorize]
        async (ContactCreateDto contactCreateDto, HttpContext context, IContactService service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
                throw new ForbiddenException();
            return Results.Ok(await service.AddContactAsync(contactCreateDto, long.Parse(userId)));
        })
        .WithName("AddContact");

        userGroup.MapDelete("/delete", [Authorize]
        async (long contactId, HttpContext context, IContactService service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
                throw new ForbiddenException();
            await service.DeleteContactAsync(contactId, long.Parse(userId));
            return Results.Ok();
        })
        .WithName("DeleteContact");

        userGroup.MapPut("/update", [Authorize]
        async (ContactDto contact, HttpContext context, IContactService service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
                throw new ForbiddenException();
            await service.UpdateContactAsync(contact, long.Parse(userId));
            return Results.Ok();
        })
        .WithName("UpdateContact");

        userGroup.MapGet("/getById", [Authorize]
        async (long contactId, HttpContext context, IContactService service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
                throw new ForbiddenException();
            var res = await service.GetContactByIdAsync(contactId, long.Parse(userId));
            return Results.Ok(res);
        })
        .WithName("GetContactById");

        userGroup.MapGet("/getAll", [Authorize]
        async (HttpContext context, IContactService service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
                throw new ForbiddenException();
            var res = await service.GetAllContactsAsync(long.Parse(userId));
            return Results.Ok(res);
        })
        .WithName("GetAllContacts");
    }
}
