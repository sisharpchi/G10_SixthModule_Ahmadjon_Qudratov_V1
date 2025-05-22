using ContactSystem.Bll.Dtos;

namespace ContactSystem.Bll.Services;

public interface IRoleService
{
    Task<List<RoleGetDto>> GetAllRolesAsync();
    Task<ICollection<UserGetDto>> GetAllUsersByRoleAsync(string role);
    Task<long> GetRoleIdAsync(string role);
    Task AddRoleAsync(RoleCreateDto roleCreateDto);
    Task DeleteRoleAsync(long id);
}