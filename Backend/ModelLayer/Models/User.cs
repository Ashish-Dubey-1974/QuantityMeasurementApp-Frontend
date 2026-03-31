namespace ModelLayer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // JWT (local) auth — nullable because Google users won't have a password
        public string? HashedPassword { get; set; }
        public string? Salt { get; set; }

        // Google OAuth
        public string? GoogleId { get; set; }
        public string? ProfilePicture { get; set; }

        // Auth provider: "local" | "google"
        public string AuthProvider { get; set; } = "local";
    }
}
