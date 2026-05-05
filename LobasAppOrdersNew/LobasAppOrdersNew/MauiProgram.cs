using Microsoft.Extensions.Logging;
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AuthApiService>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<UserApiService>();

            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomePage>();

            builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);
            builder.Services.AddSingleton<AppBiometricService>();
            return builder.Build();
        }
    }
}
