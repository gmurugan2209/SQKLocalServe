namespace SQKLocalServe.Contract.DTOs
{
    public class UserRegistrationDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int RoleId { get; set; }
    }

    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class UserUpdateDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int? StatusId { get; set; }
        public string UpdatedBy { get; set; }
    }
}