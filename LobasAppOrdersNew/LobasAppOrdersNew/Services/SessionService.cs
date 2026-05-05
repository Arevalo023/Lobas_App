using LobasAppOrdersNew.Models;

namespace LobasAppOrdersNew.Services
{
    public class SessionService
    {
        private const string UserIdKey = "user_id";
        private const string UserNameKey = "user_name";
        private const string UserEmailKey = "user_email";
        private const string AuthProviderKey = "auth_provider";
        private const string BiometricEnabledKey = "biometric_enabled";

        public async Task SaveUserSessionAsync(UserModel user)
        {
            await SecureStorage.SetAsync(UserIdKey, user.Id.ToString());
            await SecureStorage.SetAsync(UserNameKey, user.Name);
            await SecureStorage.SetAsync(UserEmailKey, user.Email);
            await SecureStorage.SetAsync(AuthProviderKey, user.AuthProvider);
            await SecureStorage.SetAsync(BiometricEnabledKey, user.BiometricEnabled.ToString());
        }

        public async Task<UserModel?> GetUserSessionAsync()
        {
            string? userIdText = await SecureStorage.GetAsync(UserIdKey);
            string? userName = await SecureStorage.GetAsync(UserNameKey);
            string? userEmail = await SecureStorage.GetAsync(UserEmailKey);
            string? authProvider = await SecureStorage.GetAsync(AuthProviderKey);
            string? biometricEnabledText = await SecureStorage.GetAsync(BiometricEnabledKey);

            if (string.IsNullOrWhiteSpace(userIdText) ||
                string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(userEmail))
            {
                return null;
            }

            int.TryParse(userIdText, out int userId);
            bool.TryParse(biometricEnabledText, out bool biometricEnabled);

            return new UserModel
            {
                Id = userId,
                Name = userName,
                Email = userEmail,
                AuthProvider = authProvider ?? "Local",
                BiometricEnabled = biometricEnabled,
                IsActive = true
            };
        }

        public async Task<bool> HasSessionAsync()
        {
            UserModel? user = await GetUserSessionAsync();

            return user != null;
        }

        public async Task<bool> IsBiometricEnabledAsync()
        {
            string? biometricEnabledText = await SecureStorage.GetAsync(BiometricEnabledKey);

            bool.TryParse(biometricEnabledText, out bool biometricEnabled);

            return biometricEnabled;
        }

        public async Task UpdateBiometricStatusAsync(bool biometricEnabled)
        {
            await SecureStorage.SetAsync(BiometricEnabledKey, biometricEnabled.ToString());
        }

        public void ClearSession()
        {
            SecureStorage.Remove(UserIdKey);
            SecureStorage.Remove(UserNameKey);
            SecureStorage.Remove(UserEmailKey);
            SecureStorage.Remove(AuthProviderKey);
            SecureStorage.Remove(BiometricEnabledKey);
        }
    }
}