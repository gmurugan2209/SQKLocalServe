using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Common;
using SQKLocalServe.Common.Logging;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogManager _logger;

        private readonly IValidator<UserRegistrationDTO> _registerValidator;
        
        private readonly IValidator<LoginRequestDTO> _loginValidator;

        public AuthController(IUserService userService, ILogManager logger, IValidator<UserRegistrationDTO> registerValidator, IValidator<LoginRequestDTO> loginValidator)
        {

            _userService = userService;
            _logger = logger;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO request)
        {
            try
            {
                _logger.LogInfo($"Registering new user: {request.Username}");

var validationResult = await _registerValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<string>.Error(validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid registration data"));
                }
                if (!await _userService.IsUsernameUniqueAsync(request.Username))
                    return BadRequest(ApiResponse<string>.Error("Username already exists"));

                if (!await _userService.IsEmailUniqueAsync(request.Email))
                    return BadRequest(ApiResponse<string>.Error("Email already exists"));

                var response = await _userService.RegisterUserAsync(request);
                return Ok(ApiResponse<UserResponseDTO>.Success(response));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
                return BadRequest(ApiResponse<string>.Error("Registration failed"));
            }
        }

        [HttpPut("update")]
       // [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO request)
        {
            try
            {
               /* var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(ApiResponse<string>.Error("User not authenticated")); */

               // var userId = int.Parse(userIdClaim.Value);
               // request.UserId = userId; // Set the UserId from the token
                request.UpdatedBy = User.Identity?.Name ?? request.UserId.ToString();

                _logger.LogInfo($"Updating user: {request.UserId}");
                var response = await _userService.UpdateUserAsync(request);

                if (response == null)
                    return BadRequest(ApiResponse<string>.Error("User not found"));

                return Ok(ApiResponse<UserResponseDTO>.Success(response));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user: {ex.Message}");
                return BadRequest(ApiResponse<string>.Error("Update failed"));
            }
        }

        [HttpGet("user/{username}")]
        //[Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound(ApiResponse<string>.Error("User not found"));

            return Ok(ApiResponse<UserResponseDTO>.Success(user));
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
[ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
{
    try
    {

        _logger.LogInfo($"Login attempt for user: {request.Username}");
        
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<string>.Error(validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid login data"));
        }
        var response = await _userService.LoginAsync(request);
        if (response == null)
            return BadRequest(ApiResponse<string>.Error("Invalid username or password"));

        return Ok(ApiResponse<LoginResponseDTO>.Success(response));
    }
    catch (Exception ex)
    {
        _logger.LogError($"Login failed: {ex.Message}");
        return BadRequest(ApiResponse<string>.Error("Login failed"));
    }
}

        [HttpGet("getuser/{id}")]
       // [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<string>.Error("User not found"));

            return Ok(ApiResponse<UserResponseDTO>.Success(user));
        }
       
        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<List<UserResponseDTO>>), StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin , Consumer")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? username,
            [FromQuery] string? email)
        {
            var users = await _userService.GetUsersAsync(username, email);
            return Ok(ApiResponse<List<UserResponseDTO>>.Success(users));
        }

       

    }
}