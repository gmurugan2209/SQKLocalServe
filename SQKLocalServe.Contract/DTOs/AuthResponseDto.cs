namespace SQKLocalServe.Contract.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserProfileDto User { get; set; }
    }
}