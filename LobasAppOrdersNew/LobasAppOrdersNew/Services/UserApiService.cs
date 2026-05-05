using System.Net.Http.Json;
using LobasAppOrdersNew.Models;

namespace LobasAppOrdersNew.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://localhost:7269/api/";

        public UserApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<ApiResponse<UserModel>> UpdateBiometricStatusAsync(int userId, bool biometricEnabled)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PatchAsync(
                    $"Users/{userId}/biometric?biometricEnabled={biometricEnabled}",
                    null
                );

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<UserModel>
                    {
                        Message = "Could not update biometric status.",
                        User = null
                    };
                }

                return new ApiResponse<UserModel>
                {
                    Message = "Biometric status updated successfully.",
                    User = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserModel>
                {
                    Message = $"Error connecting to server: {ex.Message}",
                    User = null
                };
            }
        }
    }
}