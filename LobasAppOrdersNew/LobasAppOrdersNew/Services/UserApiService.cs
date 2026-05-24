using LobasAppOrdersNew.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace LobasAppOrdersNew.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;


        public UserApiService(IConfiguration configuration)
        {
            string baseUrl = ApiEndpointResolver.GetBaseUrl(configuration);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
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
