using LobasAppOrdersNew.Views;

namespace LobasAppOrdersNew;

public partial class App : Application
{
    public App(SplashPage splashPage)
    {
        InitializeComponent();

        MainPage = splashPage;
    }
}