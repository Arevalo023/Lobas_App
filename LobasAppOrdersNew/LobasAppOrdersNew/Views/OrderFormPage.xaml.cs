using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class OrderFormPage : ContentPage
{
    private readonly OrderFormViewModel _viewModel;

    public OrderFormPage(OrderFormViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Customers.Count == 0 || _viewModel.Products.Count == 0)
        {
            await _viewModel.LoadDataAsync();
        }
    }

    private void OnRemoveCartItemInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is OrderCartItemModel item)
        {
            _viewModel.RemoveCartItem(item);
        }
    }
}