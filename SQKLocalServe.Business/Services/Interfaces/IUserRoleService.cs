namespace SQKLocalServe.Business.Services.Interfaces;

public interface IUserRoleService
{
    Task<List<string>> GetUserRolesAsync(int userId);
    Task<bool> HasRoleAsync(int userId, string role);
    Task<bool> HasAnyRoleAsync(int userId, params string[] roles);
}