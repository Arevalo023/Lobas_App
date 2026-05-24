using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class OrderDetailPage : ContentPage, IQueryAttributable
{
    private readonly OrderDetailViewModel _viewModel;

    public OrderDetailPage(OrderDetailViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("orderId", out object? orderIdValue))
        {
            string? orderIdText = orderIdValue?.ToString();

            if (int.TryParse(orderIdText, out int orderId))
            {
                await _viewModel.LoadOrderAsync(orderId);
            }
        }
    }
}