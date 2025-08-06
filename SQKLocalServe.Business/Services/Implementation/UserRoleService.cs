using Microsoft.EntityFrameworkCore;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.DataAccess;

namespace SQKLocalServe.Business.Services.Implementation;

public class UserRoleService : IUserRoleService
{
    private readonly ApplicationDbContext _context;

    public UserRoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetUserRolesAsync(int userId) => await _context.Roles
            // .Where(r => r.Users.Any(u => u.UserId == userId))
            .Select(r => r.RoleName)
            .Distinct().ToListAsync();

    public async Task<bool> HasRoleAsync(int userId, string role)
    {
        var roles = await GetUserRolesAsync(userId);
        return roles.Contains(role);
    }

    public async Task<bool> HasAnyRoleAsync(int userId, params string[] roles)
    {
        var userRoles = await GetUserRolesAsync(userId);
        return userRoles.Any(role => roles.Contains(role));
    }
}