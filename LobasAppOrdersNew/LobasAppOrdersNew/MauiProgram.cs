using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;
using LobasAppOrdersNew.ViewModels;
using LobasAppOrdersNew.Views;
using Plugin.Maui.Biometric;

namespace LobasAppOrdersNew
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            IConfigurationRoot config = LoadConfiguration();

            builder.Configuration.AddConfiguration(config);

            GoogleAuthSettings googleAuthSettings = new GoogleAuthSettings
            {
                ClientId = config["GoogleAuth:ClientId"] ?? string.Empty,
                ClientSecret = config["GoogleAuth:ClientSecret"]
                    ?? Environment.GetEnvironmentVariable("LOBAS_GOOGLE_CLIENT_SECRET")
                    ?? string.Empty
            };

            builder.Services.AddSingleton(googleAuthSettings);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Services
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<AuthApiService>();
            builder.Services.AddSingleton<UserApiService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<GoogleAuthService>();
            builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);
            builder.Services.AddSingleton<AppBiometricService>();
            builder.Services.AddSingleton<ProductApiService>();
            builder.Services.AddSingleton<CustomerApiService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<ProductsViewModel>();
            builder.Services.AddTransient<CustomersViewModel>();
            builder.Services.AddTransient<CustomerFormViewModel>();
            builder.Services.AddTransient<ProductFormViewModel>();
            builder.Services.AddTransient<OrderDetailViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<ProductsPage>();
            builder.Services.AddTransient<ProductFormPage>();
            builder.Services.AddTransient<CustomersPage>();
            builder.Services.AddTransient<CustomerFormPage>();
            builder.Services.AddTransient<ProfilePage>();

            builder.Services.AddSingleton<OrderApiService>();
            builder.Services.AddTransient<OrdersViewModel>();
            builder.Services.AddTransient<OrderDetailPage>();
            builder.Services.AddTransient<OrdersPage>();

            builder.Services.AddTransient<OrderFormViewModel>();
            builder.Services.AddTransient<OrderFormPage>();

            builder.Services.AddTransient<OrderEditViewModel>();
            builder.Services.AddTransient<OrderEditPage>();
            builder.Services.AddTransient<WelcomeViewModel>();
            builder.Services.AddTransient<WelcomePage>();

            return builder.Build();
        }

        private static IConfigurationRoot LoadConfiguration()
        {
            using Stream appSettingsStream = FileSystem
                .OpenAppPackageFileAsync("appsettings.json")
                .GetAwaiter()
                .GetResult();

            return new ConfigurationBuilder()
                .AddJsonStream(appSettingsStream)
                .Build();
        }
    }
}
