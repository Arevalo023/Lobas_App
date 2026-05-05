namespace LobasAppOrdersNew.Models
{
    public class SocialLoginRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string AuthProvider { get; set; } = string.Empty;

        public string ProviderUserId { get; set; } = string.Empty;
    }
}