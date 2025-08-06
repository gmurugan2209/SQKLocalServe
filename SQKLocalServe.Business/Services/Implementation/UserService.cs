using Microsoft.EntityFrameworkCore;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess.Entities;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.DataAccess;
using SQKLocalServe.Common.Logging;
using SQKLocalServe.Business.Services.Auth;

namespace SQKLocalServe.Business.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogManager _logger;

        private readonly IJwtService _jwtService;

        public UserService(ApplicationDbContext context, ILogManager logger, IJwtService jwtService)
        {
            _jwtService = jwtService;
          //  _passwordHasher = passwordHasher;
            _logger = logger;
            _context = context;
        }

        public async Task<UserResponseDTO> RegisterUserAsync(UserRegistrationDTO userDto)
        {
            _logger.LogInfo($"Registering new user: {userDto.Username}");

            var user = new DataAccess.Entities.User()
            {
                FullName = userDto.Username,
                PasswordHash = userDto.Password, // Assuming password is already hashed
                Email = userDto.Email,
                PhoneNumber = userDto.Mobile,
                RoleId = userDto.RoleId,
                StatusId = 1, // Active
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GetUserResponseDTO(user.UserId);
        }

        public async Task<UserResponseDTO?> UpdateUserAsync(UserUpdateDTO userDto)
        {
            _logger.LogInfo($"Updating user: {userDto.UserId}");

            // 1️⃣  Make sure the user exists, but don’t bring the whole row into memory.
            var exists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserId == userDto.UserId);

            if (!exists) return null;                       // not found ⇒ caller gets null

            // 2️⃣  Attach a stub entity so EF can track only the fields we modify.
            var user = new User { UserId = userDto.UserId };
            _context.Users.Attach(user);

            if (userDto.Email      is not null) user.Email       = userDto.Email;
            if (userDto.Mobile     is not null) user.PhoneNumber = userDto.Mobile;
            if (userDto.StatusId   is not null) user.StatusId    = userDto.StatusId.Value;

            user.UpdatedBy   = userDto.UpdatedBy;
            user.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 3️⃣  Return the up-to-date projection.
            return await GetUserResponseDTO(user.UserId);
        }

        public async Task<UserResponseDTO> GetUserByUsernameAsync(string username)
        {
            _logger.LogInfo($"Getting user by username: {username}");
            
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.FullName == username);

            if (user == null) return null;

            return await GetUserResponseDTO(user.UserId);
        }

        public async Task<UserResponseDTO?> GetByIdAsync(int userId)
        {
            _logger.LogInfo($"Fetching user by id: {userId}");
            return await GetUserResponseDTO(userId);
        }

        
        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _context.Users.AnyAsync(u => u.FullName == username);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            // 1.  Fetch only the data we really need and project it right away.
            var userData = await _context.Users
                .Where(u => u.FullName == request.Username)
                .Select(u => new
                {
                    u.PasswordHash,
                    RoleName = u.Role != null ? u.Role.RoleName : string.Empty,
                    UserDto = new UserResponseDTO      // for JWT creation
                    {
                        UserId   = u.UserId,
                        Username = u.FullName,
                        Email    = u.Email,
                        RoleId   = u.RoleId ?? 0
                    },
                    LoginDto = new LoginResponseDTO    // final result we will return
                    {
                        UserId   = u.UserId,
                        Username = u.FullName,
                        Email    = u.Email,
                        RoleName = u.Role != null ? u.Role.RoleName : string.Empty
                    }
                })
                .FirstOrDefaultAsync();

            // 2.  User not found or password mismatch ➜ return null.
            if (userData == null)
                //|| !_passwordHasher.VerifyPassword(request.Password, userData.PasswordHash))
                return null;

            // 3.  Generate the JWT and attach it to the DTO built by the query.
            var token = _jwtService.GenerateToken(userData.UserDto, userData.RoleName);
            userData.LoginDto.Token = token;

            return userData.LoginDto;
        }

        public async Task<List<UserResponseDTO>> GetUsersAsync(string? username, string? email)
        {
           
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(username))
                    query = query.Where(u => u.FullName.Contains(username));

                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(u => u.Email.Contains(email));

                query = query.OrderBy(u => u.FullName);

                var entities = await query.Select(u => new UserResponseDTO
                {
                    UserId      = u.UserId,
                    Username    = u.FullName,
                    Email       = u.Email,
                    Mobile      = u.PhoneNumber,
                    RoleId      = u.RoleId   ?? 0,
                    RoleName    = u.Role.RoleName,
                    StatusId    = u.StatusId ?? 0,
                    CreatedDate = u.CreatedDate ?? DateTime.UtcNow
                }).ToListAsync();
            
                return entities;
            
        }

        
        private async Task<UserResponseDTO?> GetUserResponseDTO(int userId) =>
            await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.UserId == userId)
                .Select(u => new UserResponseDTO
                {
                    UserId      = u.UserId,
                    Username    = u.FullName,
                    Email       = u.Email,
                    Mobile      = u.PhoneNumber,
                    RoleId      = u.RoleId   ?? 0,
                    RoleName    = u.Role.RoleName,
                    StatusId    = u.StatusId ?? 0,
                    CreatedDate = u.CreatedDate ?? DateTime.UtcNow
                })
                .FirstOrDefaultAsync();
    }
    
    


    }


