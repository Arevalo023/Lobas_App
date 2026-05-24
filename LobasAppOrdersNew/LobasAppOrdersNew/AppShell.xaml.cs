using LobasAppOrdersNew.Views;
using Microsoft.Extensions.DependencyInjection;


namespace LobasAppOrdersNew;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
        Routing.RegisterRoute(nameof(CustomersPage), typeof(CustomersPage));
        Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
        Routing.RegisterRoute(nameof(OrdersPage), typeof(OrdersPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(ProductFormPage), typeof(ProductFormPage));
        Routing.RegisterRoute(nameof(CustomerFormPage), typeof(CustomerFormPage));
        Routing.RegisterRoute(nameof(OrderFormPage), typeof(OrderFormPage));
        Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));
        Routing.RegisterRoute(nameof(OrderEditPage), typeof(OrderEditPage));

    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Cerrar sesión",
            "¿Seguro que deseas cerrar sesión?",
            "Sí",
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
}
