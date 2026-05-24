namespace LobasOrdersApi.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }

        public string AuthProvider { get; set; } = "Local";

        public string? ProviderUserId { get; set; }

        public bool BiometricEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastNameChangedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
