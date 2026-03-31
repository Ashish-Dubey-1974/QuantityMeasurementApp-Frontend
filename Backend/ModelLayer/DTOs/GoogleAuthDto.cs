namespace ModelLayer.DTOs
{
    /// <summary>
    /// DTO received from Angular when user signs in with Google.
    /// The frontend sends the Google ID token (credential) after user consent.
    /// </summary>
    public class GoogleAuthDto
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
