using LobasAppOrdersNew.Helpers;

namespace LobasAppOrdersNew.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnPinkThemeClicked(object sender, EventArgs e)
    {
        ThemeHelper.ApplyPinkTheme();

        await DisplayAlert(
            "Tema aplicado",
            "Se aplicó el tema rosa.",
            "OK"
        );
    }

    private async void OnDarkThemeClicked(object sender, EventArgs e)
    {
        ThemeHelper.ApplyDarkTheme();

        await DisplayAlert(
            "Tema aplicado",
            "Se aplicó el tema oscuro.",
            "OK"
        );
    }
}