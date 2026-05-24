using System.Globalization;
using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class ProductFormViewModel : BaseViewModel
    {
        private readonly ProductApiService _productApiService;

        private int _productId;
        private bool _isEditMode;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _price = string.Empty;
        private string _stock = string.Empty;
        private bool _isSaving;
        private string _pageHeader = "Agregar producto";
        private string _pageSubtitle = "Captura la información del producto";
        private string _saveButtonText = "Guardar producto";

        public ProductFormViewModel(ProductApiService productApiService, IDialogService dialogService, INavigationService navigationService) : base(dialogService, navigationService)
        {
            _productApiService = productApiService;

            Title = "Agregar producto";

            SaveProductCommand = new Microsoft.Maui.Controls.Command(async () => await SaveProductAsync());
            CancelCommand = new Microsoft.Maui.Controls.Command(async () => await CancelAsync());
        }

        public int ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string PageHeader
        {
            get => _pageHeader;
            set => SetProperty(ref _pageHeader, value);
        }

        public string PageSubtitle
        {
            get => _pageSubtitle;
            set => SetProperty(ref _pageSubtitle, value);
        }

        public string SaveButtonText
        {
            get => _saveButtonText;
            set => SetProperty(ref _saveButtonText, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string Stock
        {
            get => _stock;
            set => SetProperty(ref _stock, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public ICommand SaveProductCommand { get; }

        public ICommand CancelCommand { get; }

        public async Task LoadProductForEditAsync(int productId)
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                ProductModel? product = await _productApiService.GetProductByIdAsync(productId);

                if (product == null)
                {
                    await DialogService.ShowAlertAsync(
                        "Error",
                        "Product not found.",
                        "OK"
                    );

                    await NavigationService.GoBackAsync();
                    return;
                }

                ProductId = product.Id;
                IsEditMode = true;

                Title = "Editar producto";
                PageHeader = "Editar producto";
                PageSubtitle = "Actualiza la información del producto";
                SaveButtonText = "Actualizar producto";

                Name = product.Name;
                Description = product.Description ?? string.Empty;
                Price = product.Price.ToString("0.##", CultureInfo.CurrentCulture);
                Stock = product.Stock.ToString();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveProductAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                await DialogService.ShowAlertAsync(
                    "Validation",
                    "Product name is required.",
                    "OK"
                );
                return;
            }

            if (!TryParseDecimal(Price, out decimal price))
            {
                await DialogService.ShowAlertAsync(
                    "Validation",
                    "Price must be a valid number.",
                    "OK"
                );
                return;
            }

            if (!int.TryParse(Stock, out int stock))
            {
                await DialogService.ShowAlertAsync(
                    "Validation",
                    "Stock must be a valid number.",
                    "OK"
                );
                return;
            }

            if (price < 0)
            {
                await DialogService.ShowAlertAsync(
                    "Validation",
                    "Price cannot be negative.",
                    "OK"
                );
                return;
            }

            if (stock < 0)
            {
                await DialogService.ShowAlertAsync(
                    "Validation",
                    "Stock cannot be negative.",
                    "OK"
                );
                return;
            }

            try
            {
                IsBusy = true;
                IsSaving = true;

                ProductRequest request = new ProductRequest
                {
                    Name = Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    Price = price,
                    Stock = stock
                };

                bool success;

                if (IsEditMode)
                {
                    success = await _productApiService.UpdateProductAsync(ProductId, request);
                }
                else
                {
                    success = await _productApiService.CreateProductAsync(request);
                }

                if (!success)
                {
                    await DialogService.ShowAlertAsync(
                        "Error",
                        IsEditMode
                            ? "The product could not be updated."
                            : "The product could not be created.",
                        "OK"
                    );
                    return;
                }

                await DialogService.ShowAlertAsync(
                    "Success",
                    IsEditMode
                        ? "Product updated successfully."
                        : "Product created successfully.",
                    "OK"
                );

                await NavigationService.GoBackAsync();
            }
            finally
            {
                IsSaving = false;
                IsBusy = false;
            }
        }

        private static bool TryParseDecimal(string value, out decimal result)
        {
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result))
            {
                return true;
            }

            string normalizedValue = value.Replace(",", ".");

            return decimal.TryParse(
                normalizedValue,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out result
            );
        }

        private async Task CancelAsync()
        {
            await NavigationService.GoBackAsync();
        }
    }
}