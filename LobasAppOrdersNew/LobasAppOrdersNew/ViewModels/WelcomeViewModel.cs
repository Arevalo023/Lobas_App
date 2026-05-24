using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly ProductApiService _productApiService;
        private readonly CustomerApiService _customerApiService;
        private readonly OrderApiService _orderApiService;
        private readonly RealtimeNotificationService _realtimeNotificationService;

        private int _productsCount;
        private int _customersCount;
        private int _ordersCount;
        private bool _isRefreshing;

        public WelcomeViewModel(
            ProductApiService productApiService,
            CustomerApiService customerApiService,
            OrderApiService orderApiService,
            RealtimeNotificationService realtimeNotificationService,
            IDialogService dialogService,
            INavigationService navigationService)
            : base(dialogService, navigationService)
        {
            _productApiService = productApiService;
            _customerApiService = customerApiService;
            _orderApiService = orderApiService;
            _realtimeNotificationService = realtimeNotificationService;

            Title = "Dashboard";

            LoadDashboardCommand = new Microsoft.Maui.Controls.Command(async () => await LoadDashboardAsync());
            GoToProductsCommand = new Microsoft.Maui.Controls.Command(async () => await GoToProductsAsync());
            GoToCustomersCommand = new Microsoft.Maui.Controls.Command(async () => await GoToCustomersAsync());
            GoToOrdersCommand = new Microsoft.Maui.Controls.Command(async () => await GoToOrdersAsync());

            _realtimeNotificationService.ProductsChanged += OnDashboardDataChanged;
            _realtimeNotificationService.CustomersChanged += OnDashboardDataChanged;
            _realtimeNotificationService.OrdersChanged += OnDashboardDataChanged;
        }

        public int ProductsCount
        {
            get => _productsCount;
            set => SetProperty(ref _productsCount, value);
        }

        public int CustomersCount
        {
            get => _customersCount;
            set => SetProperty(ref _customersCount, value);
        }

        public int OrdersCount
        {
            get => _ordersCount;
            set => SetProperty(ref _ordersCount, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string ProductsCountText => ProductsCount.ToString();

        public string CustomersCountText => CustomersCount.ToString();

        public string OrdersCountText => OrdersCount.ToString();

        public ICommand LoadDashboardCommand { get; }

        public ICommand GoToProductsCommand { get; }

        public ICommand GoToCustomersCommand { get; }

        public ICommand GoToOrdersCommand { get; }

        public async Task LoadDashboardAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                IsRefreshing = true;

                var products = await _productApiService.GetProductsAsync();
                var customers = await _customerApiService.GetCustomersAsync();
                var orders = await _orderApiService.GetOrdersAsync();

                ProductsCount = products.Count;
                CustomersCount = customers.Count;
                OrdersCount = orders.Count;

                OnPropertyChanged(nameof(ProductsCountText));
                OnPropertyChanged(nameof(CustomersCountText));
                OnPropertyChanged(nameof(OrdersCountText));
            }
            finally
            {
                IsRefreshing = false;
                IsBusy = false;
            }
        }

        private async Task GoToProductsAsync()
        {
            await NavigationService.GoToRouteAsync("//ProductsPage");
        }

        private async Task GoToCustomersAsync()
        {
            await NavigationService.GoToRouteAsync("//CustomersPage");
        }

        private async Task GoToOrdersAsync()
        {
            await NavigationService.GoToRouteAsync("//OrdersPage");
        }

        private void OnDashboardDataChanged(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () => await LoadDashboardAsync());
        }
    }
}
