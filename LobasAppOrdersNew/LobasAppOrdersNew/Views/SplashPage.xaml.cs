using Microsoft.Extensions.DependencyInjection;

namespace LobasAppOrdersNew.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(3000);

        LoginPage loginPage = Handler!
            .MauiContext!
            .Services
            .GetRequiredService<LoginPage>();

        Application.Current!.MainPage = new NavigationPage(loginPage);
    }
}