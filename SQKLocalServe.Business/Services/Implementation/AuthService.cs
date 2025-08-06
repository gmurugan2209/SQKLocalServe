using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SQKLocalServe.Business.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserRoleService _userRoleService;
    private static readonly List<string> _invalidatedTokens = new();

    public AuthService(
        ApplicationDbContext context, 
        IConfiguration configuration,
        IUserRoleService userRoleService)
    {
        _context = context;
        _configuration = configuration;
        _userRoleService = userRoleService;
    }
    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterUserDto model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                return ApiResponse<AuthResponseDto>.Error("Password is required.");
            
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
                return ApiResponse<AuthResponseDto>.Failed("100","User already exists");

            var user = new DataAccess.Entities.User
            {
                Email = model.Email,
                FullName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                RoleId = 3, // Default to Consumer role
                PasswordHash = HashPassword(model.Password),
                StatusId = 1, // Active
                CreatedBy = "System",   
                CreatedDate = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var token = await GenerateJwtToken(user);
            return ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                User = new UserProfileDto
                {
                    Id = user.UserId,
                    Email = user.Email,
                    FirstName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Role = "Consumer"
                }
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.Failed("100","Registration failed: " + ex.Message);
        }
    }
    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                return ApiResponse<AuthResponseDto>.Error("Password is required.");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.StatusId == 1);

            if (user == null)
                return ApiResponse<AuthResponseDto>.Error("Invalid credentials");

            if (!VerifyPassword(model.Password, user.PasswordHash))
                return ApiResponse<AuthResponseDto>.Error("Invalid credentials");

            var token = await GenerateJwtToken(user);
            return ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                User = new UserProfileDto
                {
                    Id = user.UserId,
                    Email = user.Email,
                    FirstName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role?.RoleName ?? "Unknown"
                }
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.Error("Login failed: " + ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string token)
    {
        _invalidatedTokens.Add(token);
        return ApiResponse<bool>.Success(true);
    }

private async Task<string> GenerateJwtToken(SQKLocalServe.DataAccess.Entities.User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"));
    
    // Get user role
    var role = await _context.Roles
        .FirstOrDefaultAsync(r => r.RoleId == user.RoleId) 
        ?? throw new InvalidOperationException("User role not found");

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.FullName),  // Changed from Username to FirstName
        new Claim(ClaimTypes.Surname, user.FullName),
        new Claim(ClaimTypes.Role, role.RoleName),
        new Claim("UserId", user.UserId.ToString()),
        new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"] ?? "60")),
        Issuer = _configuration["Jwt:Issuer"],
        Audience = _configuration["Jwt:Audience"],
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

    private static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return null;
        
        byte[] salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        using var hmac = new HMACSHA256(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hash = hmac.ComputeHash(passwordBytes);

        byte[] hashWithSalt = new byte[salt.Length + hash.Length];
        Array.Copy(salt, 0, hashWithSalt, 0, salt.Length);
        Array.Copy(hash, 0, hashWithSalt, salt.Length, hash.Length);

        return Convert.ToBase64String(hashWithSalt);
    }
private static bool VerifyPassword(string password, string storedHash)
{
    byte[] hashWithSalt = Convert.FromBase64String(storedHash);
    byte[] salt = new byte[16];
    Array.Copy(hashWithSalt, 0, salt, 0, salt.Length);

    using var hmac = new HMACSHA256(salt);
    var passwordBytes = Encoding.UTF8.GetBytes(password);
    var computedHash = hmac.ComputeHash(passwordBytes);

    for (int i = 0; i < computedHash.Length; i++)
    {
        if (computedHash[i] != hashWithSalt[i + salt.Length])
            return false;
    }
    return true;
}
}
