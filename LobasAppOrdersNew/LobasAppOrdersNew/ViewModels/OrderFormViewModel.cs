using System.Collections.ObjectModel;
using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;

namespace LobasAppOrdersNew.ViewModels
{
    public class OrderFormViewModel : BaseViewModel
    {
        private readonly OrderApiService _orderApiService;
        private readonly CustomerApiService _customerApiService;
        private readonly ProductApiService _productApiService;

        private CustomerModel? _selectedCustomer;
        private ProductModel? _selectedProduct;
        private string _quantity = "1";
        private string _status = "Pendiente";
        private bool _isSaving;

        public OrderFormViewModel(
            OrderApiService orderApiService,
            CustomerApiService customerApiService,
            ProductApiService productApiService)
        {
            _orderApiService = orderApiService;
            _customerApiService = customerApiService;
            _productApiService = productApiService;

            Title = "Crear pedido";

            Customers = new ObservableCollection<CustomerModel>();
            Products = new ObservableCollection<ProductModel>();
            CartItems = new ObservableCollection<OrderCartItemModel>();

            LoadDataCommand = new Microsoft.Maui.Controls.Command(async () => await LoadDataAsync());
            AddProductCommand = new Microsoft.Maui.Controls.Command(async () => await AddProductAsync());
            SaveOrderCommand = new Microsoft.Maui.Controls.Command(async () => await SaveOrderAsync());
            CancelCommand = new Microsoft.Maui.Controls.Command(async () => await CancelAsync());
        }

        public ObservableCollection<CustomerModel> Customers { get; }

        public ObservableCollection<ProductModel> Products { get; }

        public ObservableCollection<OrderCartItemModel> CartItems { get; }

        public CustomerModel? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public ProductModel? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public string Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
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

        public decimal Total => CartItems.Sum(item => item.Subtotal);

        public string TotalText => $"Total: ${Total:N2}";

        public ICommand LoadDataCommand { get; }

        public ICommand AddProductCommand { get; }

        public ICommand SaveOrderCommand { get; }

        public ICommand CancelCommand { get; }

        public async Task LoadDataAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                Customers.Clear();
                Products.Clear();

                List<CustomerModel> customers = await _customerApiService.GetCustomersAsync();
                List<ProductModel> products = await _productApiService.GetProductsAsync();

                foreach (CustomerModel customer in customers)
                {
                    Customers.Add(customer);
                }

                foreach (ProductModel product in products)
                {
                    Products.Add(product);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddProductAsync()
        {
            if (SelectedProduct == null)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Validation",
                    "Please select a product.",
                    "OK"
                );
                return;
            }

            if (!int.TryParse(Quantity, out int quantity) || quantity <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Validation",
                    "Quantity must be greater than zero.",
                    "OK"
                );
                return;
            }

            if (quantity > SelectedProduct.Stock)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Validation",
                    $"Not enough stock. Available: {SelectedProduct.Stock}",
                    "OK"
                );
                return;
            }

            OrderCartItemModel? existingItem =
                CartItems.FirstOrDefault(item => item.ProductId == SelectedProduct.Id);

            if (existingItem != null)
            {
                int newQuantity = existingItem.Quantity + quantity;

                if (newQuantity > SelectedProduct.Stock)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Validation",
                        $"Not enough stock. Available: {SelectedProduct.Stock}",
                        "OK"
                    );
                    return;
                }

                CartItems.Remove(existingItem);

                existingItem.Quantity = newQuantity;

                CartItems.Add(existingItem);
            }
            else
            {
                CartItems.Add(new OrderCartItemModel
                {
                    ProductId = SelectedProduct.Id,
                    ProductName = SelectedProduct.Name,
                    UnitPrice = SelectedProduct.Price,
                    Quantity = quantity
                });
            }

            Quantity = "1";
            SelectedProduct = null;

            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(TotalText));
        }

        public void RemoveCartItem(OrderCartItemModel item)
        {
            CartItems.Remove(item);

            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(TotalText));
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

            if (CartItems.Count == 0)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Validation",
                    "Please add at least one product.",
                    "OK"
                );
                return;
            }

            try
            {
                IsBusy = true;
                IsSaving = true;

                OrderRequest request = new OrderRequest
                {
                    CustomerId = SelectedCustomer.Id,
                    Status = string.IsNullOrWhiteSpace(Status) ? "Pendiente" : Status.Trim(),
                    Details = CartItems.Select(item => new OrderDetailRequest
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
                };

                bool created = await _orderApiService.CreateOrderAsync(request);

                if (!created)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "The order could not be created.",
                        "OK"
                    );
                    return;
                }

                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    "Order created successfully.",
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