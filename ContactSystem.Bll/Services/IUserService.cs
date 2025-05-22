namespace ContactSystem.Bll.Services;

public interface IUserService
{
    Task DeleteUserByIdAsync(long userId, string role);
    Task UpdateUserRoleAsync(long userId, string role);
}