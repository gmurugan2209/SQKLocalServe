using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Business.Services.Auth
{
    public interface IJwtService
    {
        string GenerateToken(UserResponseDTO user, string roleName);
    }
}