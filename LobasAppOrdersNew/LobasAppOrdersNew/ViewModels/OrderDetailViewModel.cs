using System.Collections.ObjectModel;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class OrderDetailViewModel : BaseViewModel
    {
        private readonly OrderApiService _orderApiService;

        private int _orderId;
        private string _orderNumberText = string.Empty;
        private string _customerName = string.Empty;
        private string _dateText = string.Empty;
        private string _statusText = string.Empty;
        private string _totalText = string.Empty;

        public OrderDetailViewModel(OrderApiService orderApiService, IDialogService dialogService, INavigationService navigationService) : base(dialogService, navigationService)
        {
            _orderApiService = orderApiService;

            Title = "Order Detail";
            Details = new ObservableCollection<OrderDetailModel>();
        }

        public ObservableCollection<OrderDetailModel> Details { get; }

        public int OrderId
        {
            get => _orderId;
            set => SetProperty(ref _orderId, value);
        }

        public string OrderNumberText
        {
            get => _orderNumberText;
            set => SetProperty(ref _orderNumberText, value);
        }

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public string TotalText
        {
            get => _totalText;
            set => SetProperty(ref _totalText, value);
        }

        public async Task LoadOrderAsync(int orderId)
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                OrderModel? order = await _orderApiService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    await DialogService.ShowAlertAsync(
                        "Error",
                        "Order not found.",
                        "OK"
                    );

                    await NavigationService.GoBackAsync();
                    return;
                }

                OrderId = order.Id;
                OrderNumberText = order.OrderNumberText;
                CustomerName = order.CustomerName;
                DateText = $"Date: {order.DateText}";
                StatusText = $"Status: {order.StatusText}";
                TotalText = order.TotalText;

                Details.Clear();

                foreach (OrderDetailModel detail in order.Details)
                {
                    Details.Add(detail);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}