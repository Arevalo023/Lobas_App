using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using LobasAppOrdersNew.Models;

using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.Services
{
    public class CustomerApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IDialogService _dialogService;

        public CustomerApiService(IConfiguration configuration, IDialogService dialogService)
        {
            _dialogService = dialogService;

            string baseUrl = ApiEndpointResolver.GetBaseUrl(configuration);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<List<CustomerModel>> GetCustomersAsync()
        {
            try
            {
                List<CustomerModel>? customers =
                    await _httpClient.GetFromJsonAsync<List<CustomerModel>>("Customers");

                return customers ?? new List<CustomerModel>();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not load customers: {ex.Message}",
                    "OK"
                );

                return new List<CustomerModel>();
            }
        }

        public async Task<CustomerModel?> GetCustomerByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CustomerModel>($"Customers/{id}");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not load customer: {ex.Message}",
                    "OK"
                );

                return null;
            }
        }

        public async Task<List<CustomerModel>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetCustomersAsync();
                }

                string encodedSearchTerm = Uri.EscapeDataString(searchTerm.Trim());

                List<CustomerModel>? customers =
                    await _httpClient.GetFromJsonAsync<List<CustomerModel>>(
                        $"Customers/search?searchTerm={encodedSearchTerm}"
                    );

                return customers ?? new List<CustomerModel>();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not search customers: {ex.Message}",
                    "OK"
                );

                return new List<CustomerModel>();
            }
        }

        public async Task<bool> CreateCustomerAsync(CustomerRequest customer)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsJsonAsync("Customers", customer);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not create customer: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }

        public async Task<bool> UpdateCustomerAsync(int id, CustomerRequest customer)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PutAsJsonAsync($"Customers/{id}", customer);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not update customer: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.DeleteAsync($"Customers/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"Could not delete customer: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }
    }
}
