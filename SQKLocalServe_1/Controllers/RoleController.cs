using Microsoft.AspNetCore.Mvc;
using SQKLocalServe.Business.Services.Role;
using SQKLocalServe.Common;
using SQKLocalServe.Common.Logging;
using SQKLocalServe.Contract.Models.Role;


namespace SQKLocalServe_1.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogManager _logger;

    public RolesController(IRoleService roleService, ILogManager logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RoleResponse>>), 200)]
    public async Task<IActionResult> GetRoles()
    {
        _logger.LogInfo("Getting all roles");
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(ApiResponse<List<RoleResponse>>.Success(roles));
    }
}