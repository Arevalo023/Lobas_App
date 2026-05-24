using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class OrdersPage : ContentPage
{
    private readonly OrdersViewModel _viewModel;

    public OrdersPage(OrdersViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadOrdersAsync();
    }

    private async void OnDeleteOrderInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is OrderModel order)
        {
            await _viewModel.DeleteOrderAsync(order);
        }
    }

    private async void OnAddOrderClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(OrderFormPage));
    }

    private async void OnOrderTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border &&
            border.BindingContext is OrderModel order)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(OrderDetailPage)}?orderId={order.Id}"
            );
        }
    }

    private async void OnEditOrderInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is OrderModel order)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(OrderEditPage)}?orderId={order.Id}"
            );
        }
    }
}
