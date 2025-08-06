using Microsoft.EntityFrameworkCore;
using SQKLocalServe.Contract.Models.Role;
using SQKLocalServe.DataAccess;

namespace SQKLocalServe.Business.Services.Role;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _dbContext;

    public RoleService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<RoleResponse>> GetAllRolesAsync()
    {
        var roles = await _dbContext.Roles
            .Select(r => new RoleResponse {  Id = r.RoleId, Name = r.RoleName }).ToListAsync();
        return roles;

    }
}
