using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using LobasAppOrdersNew.Models;

namespace LobasAppOrdersNew.Services
{
    public class OrderApiService
    {
        private readonly HttpClient _httpClient;

        public OrderApiService(IConfiguration configuration)
        {
            string baseUrl = ApiEndpointResolver.GetBaseUrl(configuration);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<List<OrderModel>> GetOrdersAsync()
        {
            try
            {
                List<OrderModel>? orders =
                    await _httpClient.GetFromJsonAsync<List<OrderModel>>("Orders");

                return orders ?? new List<OrderModel>();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Could not load orders: {ex.Message}",
                    "OK"
                );

                return new List<OrderModel>();
            }
        }

        public async Task<OrderModel?> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderModel>($"Orders/{id}");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Could not load order: {ex.Message}",
                    "OK"
                );

                return null;
            }
        }

        public async Task<List<OrderModel>> SearchOrdersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetOrdersAsync();
                }

                string encodedSearchTerm = Uri.EscapeDataString(searchTerm.Trim());

                List<OrderModel>? orders =
                    await _httpClient.GetFromJsonAsync<List<OrderModel>>(
                        $"Orders/search?searchTerm={encodedSearchTerm}"
                    );

                return orders ?? new List<OrderModel>();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Could not search orders: {ex.Message}",
                    "OK"
                );

                return new List<OrderModel>();
            }
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.DeleteAsync($"Orders/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Could not delete order: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }

        public async Task<bool> CreateOrderAsync(OrderRequest order)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsJsonAsync("Orders", order);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Could not create order: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderUpdateRequest order)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PutAsJsonAsync($"Orders/{id}", order);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Could not update order: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }
    }
}
