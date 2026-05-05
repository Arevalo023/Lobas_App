using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
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

            IConfigurationRoot config = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
           .Build();

            builder.Configuration.AddConfiguration(config);

            GoogleAuthSettings googleAuthSettings = new GoogleAuthSettings
            {
                ClientId = config["GoogleAuth:ClientId"] ?? string.Empty,
                ClientSecret = config["GoogleAuth:ClientSecret"] ?? string.Empty
            };

            builder.Services.AddSingleton(googleAuthSettings);
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AuthApiService>();

            builder.Services.AddSingleton<GoogleAuthService>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<UserApiService>();

            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<GoogleAuthService>();

            builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);
            builder.Services.AddSingleton<AppBiometricService>();
            return builder.Build();
        }
    }
}
