using System.Net.Http.Json;
using LobasAppOrdersNew.Models;

namespace LobasAppOrdersNew.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://localhost:7269/api/";

        public AuthApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
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
    }
}