using LobasAppOrdersNew.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace LobasAppOrdersNew.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;


        public AuthApiService(IConfiguration configuration)
        {
            string baseUrl = ApiEndpointResolver.GetBaseUrl(configuration);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<ApiResponse<UserModel>?> LoginAsync(LoginRequest request)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Auth/login", request);

                ApiResponse<UserModel>? result =
                    await response.Content.ReadFromJsonAsync<ApiResponse<UserModel>>();

                return result;
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserModel>
                {
                    Message = $"Error al conectar con el servidor: {ex.Message}",
                    User = null
                };
            }
        }

        public async Task<ApiResponse<UserModel>?> RegisterAsync(RegisterRequest request)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Auth/register", request);

                ApiResponse<UserModel>? result =
                    await response.Content.ReadFromJsonAsync<ApiResponse<UserModel>>();

                return result;
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserModel>
                {
                    Message = $"Error al conectar con el servidor: {ex.Message}",
                    User = null
                };
            }
        }
        public async Task<ApiResponse<UserModel>?> SocialLoginAsync(SocialLoginRequest request)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Auth/social-login", request);

                ApiResponse<UserModel>? result =
                    await response.Content.ReadFromJsonAsync<ApiResponse<UserModel>>();

                return result;
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserModel>
                {
                    Message = $"Error al conectar con el servidor: {ex.Message}",
                    User = null
                };
            }
        }
    }
}
