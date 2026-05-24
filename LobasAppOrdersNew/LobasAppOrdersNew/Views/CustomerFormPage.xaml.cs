using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class CustomerFormPage : ContentPage, IQueryAttributable
{
    private readonly CustomerFormViewModel _viewModel;

    public CustomerFormPage(CustomerFormViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("customerId", out object? customerIdValue))
        {
            string? customerIdText = customerIdValue?.ToString();

            if (int.TryParse(customerIdText, out int customerId))
            {
                await _viewModel.LoadCustomerForEditAsync(customerId);
            }
        }
    }
}