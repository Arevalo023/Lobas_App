using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Views;
using LobasAppOrdersNew.Helpers;
using Microsoft.Extensions.DependencyInjection;


namespace LobasAppOrdersNew;

public partial class AppShell : Shell
{
    private readonly RealtimeNotificationService _realtimeNotificationService;

    public AppShell(RealtimeNotificationService realtimeNotificationService)
    {
        InitializeComponent();

        _realtimeNotificationService = realtimeNotificationService;

        Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
        Routing.RegisterRoute(nameof(CustomersPage), typeof(CustomersPage));
        Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
        Routing.RegisterRoute(nameof(OrdersPage), typeof(OrdersPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(ProductFormPage), typeof(ProductFormPage));
        Routing.RegisterRoute(nameof(CustomerFormPage), typeof(CustomerFormPage));
        Routing.RegisterRoute(nameof(CustomerAddressesPage), typeof(CustomerAddressesPage));
        Routing.RegisterRoute(nameof(OrderFormPage), typeof(OrderFormPage));
        Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));
        Routing.RegisterRoute(nameof(OrderEditPage), typeof(OrderEditPage));

        ThemeHelper.ThemeChanged += OnThemeChanged;
        RefreshThemeIcons();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _realtimeNotificationService.StartAsync();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Cerrar sesi\u00f3n",
            "\u00bfSeguro que deseas cerrar sesi\u00f3n?",
            "S\u00ed",
            "Cancelar"
        );

        if (!confirm)
        {
            return;
        }

        LoginPage loginPage = Handler!
            .MauiContext!
            .Services
            .GetRequiredService<LoginPage>();

        Application.Current!.Windows[0].Page = new NavigationPage(loginPage);

    }

    private async void OnProfileHeaderTapped(object sender, TappedEventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync(nameof(ProfilePage));
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(RefreshThemeIcons);
    }

    private void RefreshThemeIcons()
    {
        HomeFlyoutItem.Icon = GetImageSource("HomeIconSource");
        CustomersFlyoutItem.Icon = GetImageSource("CustomersIconSource");
        ProductsFlyoutItem.Icon = GetImageSource("ProductsIconSource");
        OrdersFlyoutItem.Icon = GetImageSource("OrdersIconSource");
        SettingsFlyoutItem.Icon = GetImageSource("SettingsIconSource");
        AboutFlyoutItem.Icon = GetImageSource("AboutIconSource");

        ProfileIconImage.Source = GetImageSource("ProfileIconSource");
        LogoutIconImage.Source = GetImageSource("LogoutIconSource");
    }

    private static ImageSource GetImageSource(string resourceKey)
    {
        string fileName = Application.Current!.Resources[resourceKey]?.ToString() ?? string.Empty;

        return ImageSource.FromFile(fileName);
    }
}
