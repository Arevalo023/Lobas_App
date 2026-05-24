using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class ProductsPage : ContentPage
{
    private readonly ProductsViewModel _viewModel;

    public ProductsPage(ProductsViewModel viewModel)
    {
        InitializeComponent();
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadProductsAsync();
    }
    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProductFormPage));
    }

    private async void OnEditProductInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is ProductModel product)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(ProductFormPage)}?productId={product.Id}"
            );
        }
    }

    private async void OnDeleteProductInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is ProductModel product)
        {
            await _viewModel.DeleteProductAsync(product);
        }
    }
}