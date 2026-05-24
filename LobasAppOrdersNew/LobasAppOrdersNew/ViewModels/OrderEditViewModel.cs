using System.Collections.ObjectModel;
using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;

namespace LobasAppOrdersNew.ViewModels
{
    public class OrderEditViewModel : BaseViewModel
    {
        private readonly OrderApiService _orderApiService;
        private readonly CustomerApiService _customerApiService;

        private int _orderId;
        private CustomerModel? _selectedCustomer;
        private string _status = string.Empty;
        private bool _isSaving;

        public OrderEditViewModel(
            OrderApiService orderApiService,
            CustomerApiService customerApiService)
        {
            _orderApiService = orderApiService;
            _customerApiService = customerApiService;

            Title = "Editar pedido";

            Customers = new ObservableCollection<CustomerModel>();

            SaveOrderCommand = new Microsoft.Maui.Controls.Command(async () => await SaveOrderAsync());
            CancelCommand = new Microsoft.Maui.Controls.Command(async () => await CancelAsync());
        }

        public ObservableCollection<CustomerModel> Customers { get; }

        public int OrderId
        {
            get => _orderId;
            set => SetProperty(ref _orderId, value);
        }

        public CustomerModel? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        public List<string> StatusOptions { get; } = new()
{
    "Pendiente",
    "Finalizado"
};

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public ICommand SaveOrderCommand { get; }

        public ICommand CancelCommand { get; }

        public async Task LoadOrderForEditAsync(int orderId)
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

                OrderModel? order = await _orderApiService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "Order not found.",
                        "OK"
                    );

                    await Shell.Current.GoToAsync("..");
                    return;
                }

                OrderId = order.Id;
                Status = order.Status;

                SelectedCustomer = Customers.FirstOrDefault(c => c.Id == order.CustomerId);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveOrderAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (SelectedCustomer == null)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Validation",
                    "Please select a customer.",
                    "OK"
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(Status))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Validation",
                    "Status is required.",
                    "OK"
                );
                return;
            }

            try
            {
                IsBusy = true;
                IsSaving = true;

                OrderUpdateRequest request = new OrderUpdateRequest
                {
                    CustomerId = SelectedCustomer.Id,
                    Status = Status.Trim()
                };

                bool updated = await _orderApiService.UpdateOrderAsync(OrderId, request);

                if (!updated)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "The order could not be updated.",
                        "OK"
                    );
                    return;
                }

                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    "Order updated successfully.",
                    "OK"
                );

                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsSaving = false;
                IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}