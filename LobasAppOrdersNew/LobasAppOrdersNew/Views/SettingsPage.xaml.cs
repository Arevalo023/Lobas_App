using LobasAppOrdersNew.Helpers;

namespace LobasAppOrdersNew.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        UpdateCurrentThemeLabel();
    }

    private async void OnPinkThemeClicked(object sender, EventArgs e)
    {
        ThemeHelper.ApplyPinkTheme();
        UpdateCurrentThemeLabel();

        await DisplayAlert(
            "Tema aplicado",
            "Se aplicó el estilo Lobas claro.",
            "OK"
        );
    }

    private async void OnDarkThemeClicked(object sender, EventArgs e)
    {
        ThemeHelper.ApplyDarkTheme();
        UpdateCurrentThemeLabel();

        await DisplayAlert(
            "Tema aplicado",
            "Se aplicó el estilo Nocturno profesional.",
            "OK"
        );
    }

    private void UpdateCurrentThemeLabel()
    {
        CurrentThemeLabel.Text =
            $"Estilo actual: {ThemeHelper.GetThemeDisplayName(ThemeHelper.CurrentThemeKey)}";
    }
}
