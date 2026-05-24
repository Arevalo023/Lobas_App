namespace LobasOrdersApi.DTOs
{
    public class UserCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class UserUpdateDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool BiometricEnabled { get; set; }
    }

    public class UserNameUpdateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class SocialLoginDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string AuthProvider { get; set; } = string.Empty;

        public string ProviderUserId { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string AuthProvider { get; set; } = string.Empty;

        public string? ProviderUserId { get; set; }

        public bool BiometricEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastNameChangedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
