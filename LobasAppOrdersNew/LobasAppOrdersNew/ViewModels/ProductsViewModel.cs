
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LobasAppOrdersNew.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private readonly ProductApiService _productApiService;

        private string _searchText = string.Empty;
        private bool _isRefreshing;

        public ProductsViewModel(ProductApiService productApiService)
        {
            _productApiService = productApiService;

            Title = "Products";

            Products = new ObservableCollection<ProductModel>();

            LoadProductsCommand = new Microsoft.Maui.Controls.Command(async () => await LoadProductsAsync());
            RefreshCommand = new Microsoft.Maui.Controls.Command(async () => await RefreshProductsAsync());
        }

        public ObservableCollection<ProductModel> Products { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = SearchProductsAsync();
                }
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand LoadProductsCommand { get; }

        public ICommand RefreshCommand { get; }

        public async Task LoadProductsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                Products.Clear();

                List<ProductModel> products = await _productApiService.GetProductsAsync();

                foreach (ProductModel product in products)
                {
                    Products.Add(product);
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private async Task RefreshProductsAsync()
        {
            IsRefreshing = true;
            SearchText = string.Empty;
            await LoadProductsAsync();
        }

        private async Task SearchProductsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                Products.Clear();

                List<ProductModel> products =
                    await _productApiService.SearchProductsAsync(SearchText);

                foreach (ProductModel product in products)
                {
                    Products.Add(product);
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        public async Task DeleteProductAsync(ProductModel product)
        {
            if (IsBusy)
            {
                return;
            }

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Eliminar producto",
                $"¿Seguro que deseas eliminar \"{product.Name}\"?",
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

                bool deleted = await _productApiService.DeleteProductAsync(product.Id);

                if (!deleted)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "The product could not be deleted.",
                        "OK"
                    );
                    return;
                }

                Products.Remove(product);

                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    "Product deleted successfully.",
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