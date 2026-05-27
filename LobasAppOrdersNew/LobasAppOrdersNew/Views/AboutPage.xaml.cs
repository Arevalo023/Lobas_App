using LobasAppOrdersNew.Helpers;

namespace LobasAppOrdersNew.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ThemeHelper.ThemeChanged += OnThemeChanged;
        RefreshAboutIcon();
    }

    protected override void OnDisappearing()
    {
        ThemeHelper.ThemeChanged -= OnThemeChanged;

        base.OnDisappearing();
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(RefreshAboutIcon);
    }

    private void RefreshAboutIcon()
    {
        string iconSource = Application.Current!.Resources["AboutIconSource"]?.ToString() ?? "about.png";

        AboutIconImage.Source = ImageSource.FromFile(iconSource);
    }
}
