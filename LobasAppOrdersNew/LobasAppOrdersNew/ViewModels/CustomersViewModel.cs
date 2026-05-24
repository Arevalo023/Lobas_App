using System.Collections.ObjectModel;
using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;

namespace LobasAppOrdersNew.ViewModels
{
    public class CustomersViewModel : BaseViewModel
    {
        private readonly CustomerApiService _customerApiService;

        private string _searchText = string.Empty;
        private bool _isRefreshing;

        public CustomersViewModel(CustomerApiService customerApiService)
        {
            _customerApiService = customerApiService;

            Title = "Customers";

            Customers = new ObservableCollection<CustomerModel>();

            LoadCustomersCommand = new Microsoft.Maui.Controls.Command(async () => await LoadCustomersAsync());
            RefreshCommand = new Microsoft.Maui.Controls.Command(async () => await RefreshCustomersAsync());
        }

        public ObservableCollection<CustomerModel> Customers { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = SearchCustomersAsync();
                }
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand LoadCustomersCommand { get; }

        public ICommand RefreshCommand { get; }

        public async Task LoadCustomersAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                Customers.Clear();

                List<CustomerModel> customers = await _customerApiService.GetCustomersAsync();

                foreach (CustomerModel customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private async Task RefreshCustomersAsync()
        {
            IsRefreshing = true;
            SearchText = string.Empty;
            await LoadCustomersAsync();
        }

        private async Task SearchCustomersAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                Customers.Clear();

                List<CustomerModel> customers =
                    await _customerApiService.SearchCustomersAsync(SearchText);

                foreach (CustomerModel customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        public async Task DeleteCustomerAsync(CustomerModel customer)
        {
            if (IsBusy)
            {
                return;
            }

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Eliminar cliente",
                $"¿Seguro que deseas eliminar \"{customer.Name}\"?",
                "Sí",
                "Cancelar"
            );

            if (!confirm)
            {
                return;
            }

            try
            {
                IsBusy = true;

                bool deleted = await _customerApiService.DeleteCustomerAsync(customer.Id);

                if (!deleted)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "The customer could not be deleted.",
                        "OK"
                    );
                    return;
                }

                Customers.Remove(customer);

                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    "Customer deleted successfully.",
                    "OK"
                );
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}