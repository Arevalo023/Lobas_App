using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class ProductFormPage : ContentPage, IQueryAttributable
{
    private readonly ProductFormViewModel _viewModel;

    public ProductFormPage(ProductFormViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("productId", out object? productIdValue))
        {
            string? productIdText = productIdValue?.ToString();

            if (int.TryParse(productIdText, out int productId))
            {
                await _viewModel.LoadProductForEditAsync(productId);
            }
        }
    }
}