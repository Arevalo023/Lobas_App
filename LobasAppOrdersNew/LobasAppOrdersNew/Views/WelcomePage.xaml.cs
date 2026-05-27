using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel _viewModel;

    public WelcomePage(WelcomeViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ThemeHelper.ThemeChanged += OnThemeChanged;
        RefreshThemeIcons();

        await _viewModel.LoadDashboardAsync();
    }

    protected override void OnDisappearing()
    {
        ThemeHelper.ThemeChanged -= OnThemeChanged;

        base.OnDisappearing();
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(RefreshThemeIcons);
    }

    private void RefreshThemeIcons()
    {
        ProductsIconImage.Source = GetImageSource("ProductsIconSource");
        CustomersIconImage.Source = GetImageSource("CustomersIconSource");
        OrdersIconImage.Source = GetImageSource("OrdersIconSource");
    }

    private static ImageSource GetImageSource(string resourceKey)
    {
        string fileName = Application.Current!.Resources[resourceKey]?.ToString() ?? string.Empty;

        return ImageSource.FromFile(fileName);
    }
}
