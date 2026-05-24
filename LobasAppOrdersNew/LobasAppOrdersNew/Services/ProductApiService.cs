using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using LobasAppOrdersNew.Models;

using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IDialogService _dialogService;

        public ProductApiService(IConfiguration configuration, IDialogService dialogService)
        {
            _dialogService = dialogService;

            string baseUrl = ApiEndpointResolver.GetBaseUrl(configuration);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<List<ProductModel>> GetProductsAsync()
        {
            try
            {
                List<ProductModel>? products =
                    await _httpClient.GetFromJsonAsync<List<ProductModel>>("Products");

                return products ?? new List<ProductModel>();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not load products: {ex.Message}",
                    "OK"
                );

                return new List<ProductModel>();
            }
        }

        public async Task<List<ProductModel>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetProductsAsync();
                }

                string encodedSearchTerm = Uri.EscapeDataString(searchTerm.Trim());

                List<ProductModel>? products =
                    await _httpClient.GetFromJsonAsync<List<ProductModel>>(
                        $"Products/search?searchTerm={encodedSearchTerm}"
                    );

                return products ?? new List<ProductModel>();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not search products: {ex.Message}",
                    "OK"
                );

                return new List<ProductModel>();
            }
        }

        public async Task<bool> CreateProductAsync(ProductRequest product)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsJsonAsync("Products", product);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not create product: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }
        public async Task<ProductModel?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductModel>($"Products/{id}");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not load product: {ex.Message}",
                    "OK"
                );

                return null;
            }
        }

        public async Task<bool> UpdateProductAsync(int id, ProductRequest product)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PutAsJsonAsync($"Products/{id}", product);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not update product: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.DeleteAsync($"Products/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not delete product: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }

    }
}
