using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Views;

namespace LobasAppOrdersNew;

public partial class App : Application
{
    public App(SplashPage splashPage)
    {
        InitializeComponent();

        ThemeHelper.ApplySavedTheme();

        MainPage = splashPage;
    }
}
