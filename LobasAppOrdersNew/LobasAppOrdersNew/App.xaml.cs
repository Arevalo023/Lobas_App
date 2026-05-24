using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Views;

namespace LobasAppOrdersNew;

public partial class App : Application
{
    private readonly SplashPage _splashPage;

    public App(SplashPage splashPage)
    {
        InitializeComponent();

        ThemeHelper.ApplySavedTheme();

        _splashPage = splashPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_splashPage);
    }
}
