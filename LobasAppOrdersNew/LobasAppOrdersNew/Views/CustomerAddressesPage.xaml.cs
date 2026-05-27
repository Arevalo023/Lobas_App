using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class CustomerAddressesPage : ContentPage, IQueryAttributable
{
    private readonly CustomerAddressesViewModel _viewModel;

    public CustomerAddressesPage(CustomerAddressesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("customerId", out object? customerIdValue))
        {
            return;
        }

        string? customerIdText = customerIdValue?.ToString();

        if (!int.TryParse(customerIdText, out int customerId))
        {
            return;
        }

        string customerName = query.TryGetValue("customerName", out object? customerNameValue)
            ? Uri.UnescapeDataString(customerNameValue?.ToString() ?? string.Empty)
            : string.Empty;

        await _viewModel.InitializeAsync(customerId, customerName);
    }

    private async void OnDeleteAddressInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is AddressModel address)
        {
            await _viewModel.DeleteAddressAsync(address);
        }
    }

    private void OnEditAddressInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.BindingContext is AddressModel address)
        {
            _viewModel.BeginEditAddress(address);
        }
    }
}
