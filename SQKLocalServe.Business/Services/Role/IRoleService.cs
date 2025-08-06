using SQKLocalServe.Contract.Models.Role;

namespace SQKLocalServe.Business.Services.Role;

public interface IRoleService
{
    Task<List<RoleResponse>> GetAllRolesAsync();
}
