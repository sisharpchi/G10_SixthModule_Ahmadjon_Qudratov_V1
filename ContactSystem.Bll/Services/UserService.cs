using ContactSystem.Core.Errors;
using ContactSystem.Dal;
using ContactSystem.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactSystem.Bll.Services;

public class UserService(MainContext mainContext) : IUserService
{
    public async Task UpdateUserRoleAsync(long userId, string userRole)
    {
        var user = await SelectUserByIdAync(userId);
        var role = await mainContext.UserRoles.FindAsync(userRole);
        if (role is null)
            throw new EntityNotFoundException($"Role : {userRole} not found");
        user.RoleId = role.Id;
        mainContext.Users.Update(user);
        await mainContext.SaveChangesAsync();
    } 
    public async Task DeleteUserByIdAsync(long userId, string userRole)
    {
        if (userRole == "SuperAdmin")
            await DeleteDbUserByIdAsync(userId);
        else if (userRole == "Admin")
        {
            var user = await SelectUserByIdAync(userId);
            if (user.Role.Name == "User")
                await DeleteDbUserByIdAsync(userId);
            else
                throw new NotAllowedException("Admin can not delete Admin or SuperAdmin");
        }
    }


    private async Task<User> SelectUserByIdAync(long id)
    {
        var user = await mainContext.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.UserId == id);
        if (user is null)
            throw new EntityNotFoundException($"Entity with {id} not found");
        return user;
    }
    private async Task DeleteDbUserByIdAsync(long userId)
    {
        var user = await SelectUserByIdAync(userId);
        mainContext.Users.Remove(user);
        await mainContext.SaveChangesAsync();
    }
    private async Task<long> InsertUserAync(User user)
    {
        await mainContext.Users.AddAsync(user);
        await mainContext.SaveChangesAsync();
        return user.UserId;
    }
}