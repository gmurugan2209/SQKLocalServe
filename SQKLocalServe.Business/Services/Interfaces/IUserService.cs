using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Business.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> RegisterUserAsync(UserRegistrationDTO userDto);
    Task<UserResponseDTO> UpdateUserAsync(UserUpdateDTO userDto);
    Task<UserResponseDTO> GetUserByUsernameAsync(string username);
    Task<bool> IsUsernameUniqueAsync(string username);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
    
    Task<UserResponseDTO?> GetByIdAsync(int userId);
    
    Task<List<UserResponseDTO>> GetUsersAsync(string? username, string? email);

        
    }
