using SQKLocalServe.Common;

namespace SQKLocalServe.Business.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<bool>> LogoutAsync(string token);
}
