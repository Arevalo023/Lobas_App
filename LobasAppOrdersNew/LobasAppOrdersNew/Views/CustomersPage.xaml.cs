using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class CustomersPage : ContentPage
{
    private readonly CustomersViewModel _viewModel;

    public CustomersPage(CustomersViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadCustomersAsync();
    }

    private async void OnAddCustomerClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CustomerFormPage));
    }

    private async void OnEditCustomerInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is CustomerModel customer)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(CustomerFormPage)}?customerId={customer.Id}"
            );
        }
    }

    private async void OnDeleteCustomerInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is CustomerModel customer)
        {
            await _viewModel.DeleteCustomerAsync(customer);
        }
    }
}