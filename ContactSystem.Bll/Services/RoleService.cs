using ContactSystem.Bll.Dtos;
using ContactSystem.Core.Errors;
using ContactSystem.Dal;
using ContactSystem.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ContactSystem.Bll.Services;

public class RoleService(IMemoryCache cache, MainContext mainContext) : IRoleService
{
    private const string _cacheKey = "roles_list";

    public async Task AddRoleAsync(RoleCreateDto roleCreateDto)
    {
        var roleDto = new Role
        {
            Name = roleCreateDto.Name,
            Description = roleCreateDto.Description,
        };
        await InsertRoleAsync(roleDto);
    }

    public async Task DeleteRoleAsync(long id)
    {
        await RemoveRoleAsync(id);
    }

    public async Task<List<RoleGetDto>> GetAllRolesAsync()
    {
        List<Role> roles;
        if (cache.TryGetValue(_cacheKey, out List<Role> cachedRoles))
            roles = cachedRoles!;
        else
            roles = await SelectAllRolesAsync();

        cache.Set(_cacheKey, roles, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
            SlidingExpiration = TimeSpan.FromMinutes(15)
        });

        return roles.Select(Converter).ToList();
    }

    public async Task<ICollection<UserGetDto>> GetAllUsersByRoleAsync(string role)
    {
        var users = await SelectAllUsersByRoleAsync(role);
        return users.Select(user => Converter(user)).ToList();
    }
    
    public async Task<long> GetRoleIdAsync(string role) => await SelectRoleIdAsync(role);

    private RoleGetDto Converter(Role role)
    {
        return new RoleGetDto
        {
            Description = role.Description,
            Id = role.Id,
            Name = role.Name,
        };
    }
    private UserGetDto Converter(User user)
    {
        return new UserGetDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            UserId = user.UserId,
            UserName = user.UserName,
            Role = user.Role.Name,
        };
    }

    private async Task<List<Role>> SelectAllRolesAsync()
    {
        return await mainContext.UserRoles.ToListAsync();
    }

    private async Task<ICollection<User>> SelectAllUsersByRoleAsync(string role)
    {
        var foundRole = await mainContext.UserRoles.Include(u => u.Users).FirstOrDefaultAsync(u => u.Name == role);
        if (foundRole is null)
            throw new EntityNotFoundException(role);
        return foundRole.Users;
    }

    private async Task<long> SelectRoleIdAsync(string role)
    {
        var foundRole = await mainContext.UserRoles.FirstOrDefaultAsync(u => u.Name == role);
        if (foundRole is null)
            throw new EntityNotFoundException(role + " - not found");
        return foundRole.Id;
    }
    private async Task<long> InsertRoleAsync(Role role)
    {
        await mainContext.UserRoles.AddAsync(role);
        await mainContext.SaveChangesAsync();
        return role.Id;
    }
    private async Task RemoveRoleAsync(long id)
    {
        var role = await mainContext.UserRoles.FindAsync(id);
        if (role is null)
            throw new EntityNotFoundException($"Role with {id} not found");
        mainContext.UserRoles.Remove(role);
        await mainContext.SaveChangesAsync();
    }
}