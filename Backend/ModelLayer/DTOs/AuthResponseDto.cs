namespace ModelLayer.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
