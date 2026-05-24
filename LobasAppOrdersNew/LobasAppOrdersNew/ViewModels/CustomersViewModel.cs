using System.Collections.ObjectModel;
using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class CustomersViewModel : BaseViewModel
    {
        private readonly CustomerApiService _customerApiService;
        private readonly RealtimeNotificationService _realtimeNotificationService;

        private string _searchText = string.Empty;
        private bool _isRefreshing;

        public CustomersViewModel(
            CustomerApiService customerApiService,
            RealtimeNotificationService realtimeNotificationService,
            IDialogService dialogService,
            INavigationService navigationService) : base(dialogService, navigationService)
        {
            _customerApiService = customerApiService;
            _realtimeNotificationService = realtimeNotificationService;

            Title = "Customers";

            Customers = new ObservableCollection<CustomerModel>();

            LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
            RefreshCommand = new Command(async () => await RefreshCustomersAsync());

            _realtimeNotificationService.CustomersChanged += OnCustomersChanged;
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

            bool confirm = await DialogService.ShowConfirmationAsync(
                "Eliminar cliente",
                $"\u00bfSeguro que deseas eliminar \"{customer.Name}\"?",
                "S\u00ed",
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
                    return;
                }

                Customers.Remove(customer);

                await DialogService.ShowAlertAsync(
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

        private void OnCustomersChanged(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    await SearchCustomersAsync();
                    return;
                }

                await LoadCustomersAsync();
            });
        }
    }
}
