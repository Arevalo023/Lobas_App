using System.Collections.ObjectModel;
using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private readonly OrderApiService _orderApiService;

        private string _searchText = string.Empty;
        private bool _isRefreshing;

        public OrdersViewModel(OrderApiService orderApiService, IDialogService dialogService, INavigationService navigationService) : base(dialogService, navigationService)
        {
            _orderApiService = orderApiService;

            Title = "Orders";

            Orders = new ObservableCollection<OrderModel>();

            LoadOrdersCommand = new Microsoft.Maui.Controls.Command(async () => await LoadOrdersAsync());
            RefreshCommand = new Microsoft.Maui.Controls.Command(async () => await RefreshOrdersAsync());
        }

        public ObservableCollection<OrderModel> Orders { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = SearchOrdersAsync();
                }
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand LoadOrdersCommand { get; }

        public ICommand RefreshCommand { get; }

        public async Task LoadOrdersAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                Orders.Clear();

                List<OrderModel> orders = await _orderApiService.GetOrdersAsync();

                foreach (OrderModel order in orders)
                {
                    Orders.Add(order);
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private async Task RefreshOrdersAsync()
        {
            IsRefreshing = true;
            SearchText = string.Empty;
            await LoadOrdersAsync();
        }

        private async Task SearchOrdersAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                Orders.Clear();

                List<OrderModel> orders =
                    await _orderApiService.SearchOrdersAsync(SearchText);

                foreach (OrderModel order in orders)
                {
                    Orders.Add(order);
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        public async Task DeleteOrderAsync(OrderModel order)
        {
            if (IsBusy)
            {
                return;
            }

            bool confirm = await DialogService.ShowConfirmationAsync(
                "Eliminar pedido",
                $"¿Seguro que deseas eliminar el pedido #{order.Id}?",
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

                bool deleted = await _orderApiService.DeleteOrderAsync(order.Id);

                if (!deleted)
                {
                    await DialogService.ShowAlertAsync(
                        "Error",
                        "The order could not be deleted.",
                        "OK"
                    );
                    return;
                }

                Orders.Remove(order);

                await DialogService.ShowAlertAsync(
                    "Success",
                    "Order deleted successfully.",
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