namespace SQKLocalServe.Contract.Models.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int RoleId { get; set; }
    }

    public class RegisterResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}