using System.Net.Http.Json;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LobasAppOrdersNew.Services
{
    public class AddressApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IDialogService _dialogService;

        public AddressApiService(IConfiguration configuration, IDialogService dialogService)
        {
            _dialogService = dialogService;

            string baseUrl = ApiEndpointResolver.GetBaseUrl(configuration);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<List<AddressModel>> GetAddressesByCustomerIdAsync(int customerId)
        {
            try
            {
                List<AddressModel>? addresses =
                    await _httpClient.GetFromJsonAsync<List<AddressModel>>(
                        $"Addresses/customer/{customerId}"
                    );

                return addresses ?? new List<AddressModel>();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"No se pudieron cargar las direcciones: {ex.Message}",
                    "OK"
                );

                return new List<AddressModel>();
            }
        }

        public async Task<bool> CreateAddressAsync(AddressRequest address)
        {
            try
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsJsonAsync("Addresses", address);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"No se pudo crear la direcci\u00f3n: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Addresses/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Error",
                    $"No se pudo eliminar la direcci\u00f3n: {ex.Message}",
                    "OK"
                );

                return false;
            }
        }
    }
}
