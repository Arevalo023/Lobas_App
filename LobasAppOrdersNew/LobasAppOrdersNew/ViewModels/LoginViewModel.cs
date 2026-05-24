using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;
using LobasAppOrdersNew.Views;
using Microsoft.Extensions.DependencyInjection;
namespace LobasAppOrdersNew.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthApiService _authApiService;
        private readonly SessionService _sessionService;
        private readonly AppBiometricService _biometricService;
        private readonly GoogleAuthService _googleAuthService;
        private readonly IServiceProvider _serviceProvider;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isBiometricButtonVisible;
        private bool _isPasswordHidden = true;
        private string _passwordToggleText = "Show";
        private bool _isLoginLoading;
        private bool _isGoogleLoading;

        public LoginViewModel(
        AuthApiService authApiService,
        SessionService sessionService,
        AppBiometricService biometricService,
        GoogleAuthService googleAuthService,
        IServiceProvider serviceProvider,
        IDialogService dialogService,
        INavigationService navigationService)
            : base(dialogService, navigationService)
        {
            _authApiService = authApiService;
            _sessionService = sessionService;
            _biometricService = biometricService;
            _googleAuthService = googleAuthService;
            _serviceProvider = serviceProvider;

            Title = "Login";

            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () => await GoToRegisterAsync());
            BiometricLoginCommand = new Command(async () => await BiometricLoginAsync());
            GoogleLoginCommand = new Command(async () => await GoogleLoginAsync());
            TogglePasswordVisibilityCommand = new Command(TogglePasswordVisibility);    
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    _ = CheckBiometricAvailabilityAsync();
                }
            }
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsBiometricButtonVisible
        {
            get => _isBiometricButtonVisible;
            set => SetProperty(ref _isBiometricButtonVisible, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        public ICommand BiometricLoginCommand { get; }

        public ICommand GoogleLoginCommand { get; }

        public ICommand TogglePasswordVisibilityCommand { get; }


        public bool IsPasswordHidden
        {
            get => _isPasswordHidden;
            set => SetProperty(ref _isPasswordHidden, value);
        }

        public string PasswordToggleText
        {
            get => _passwordToggleText;
            set => SetProperty(ref _passwordToggleText, value);
        }
        private async Task LoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                await DialogService.ShowAlertAsync("Validation", "Email is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                await DialogService.ShowAlertAsync("Validation", "Password is required.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                IsLoginLoading = true;

                LoginRequest request = new LoginRequest
                {
                    Email = Email.Trim(),
                    Password = Password
                };

                ApiResponse<UserModel>? response = await _authApiService.LoginAsync(request);

                if (response == null)
                {
                    await DialogService.ShowAlertAsync("Error", "No response from server.", "OK");
                    return;
                }

                if (response.User == null)
                {
                    await DialogService.ShowAlertAsync("Login failed", response.Message, "OK");
                    return;
                }

                await _sessionService.SaveUserSessionAsync(response.User);

                await DialogService.ShowAlertAsync(
                    "Welcome",
                    $"Hello {response.User.Name}!",
                    "OK"
                );

                await GoToAppShellAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsLoginLoading = false;
                
            }
        }
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
            PasswordToggleText = IsPasswordHidden ? "Show" : "Hide";
        }
        private async Task GoogleLoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                                IsGoogleLoading = true;
                GoogleUserInfo? googleUser = await _googleAuthService.LoginWithGoogleAsync();

                if (googleUser == null)
                {
                    await DialogService.ShowAlertAsync(
                        "Google login",
                        "Google sign-in was cancelled or was not completed. Please try again.",
                        "OK"
                    );

                    return;
                }
                SocialLoginRequest request = new SocialLoginRequest
                {
                    Name = googleUser.Name,
                    Email = googleUser.Email,
                    AuthProvider = "Google",
                    ProviderUserId = googleUser.Id
                };

                ApiResponse<UserModel>? response = await _authApiService.SocialLoginAsync(request);

                if (response == null)
                {
                    await DialogService.ShowAlertAsync(
                        "Error",
                        "No response from server.",
                        "OK"
                    );
                    return;
                }

                if (response.User == null)
                {
                    await DialogService.ShowAlertAsync(
                        "Google login failed",
                        response.Message,
                        "OK"
                    );
                    return;
                }

                response.User.BiometricEnabled = false;

                await _sessionService.SaveUserSessionAsync(response.User);

                await DialogService.ShowAlertAsync(
                    "Welcome",
                    $"Hello {response.User.Name}!",
                    "OK"
                );

                await GoToAppShellAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(
                    "Error",
                    ex.Message,
                    "OK"
                );
            }
            finally
            {
                IsGoogleLoading = false;
                IsBusy = false;
                
            }
        }

        private async Task BiometricLoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Email))
                {
                    await DialogService.ShowAlertAsync(
                        "Validation",
                        "Please enter your email first.",
                        "OK"
                    );
                    return;
                }

                UserModel? user = await _sessionService.GetUserSessionAsync();

                if (user == null)
                {
                    await DialogService.ShowAlertAsync(
                        "No session",
                        "Please login with email and password first.",
                        "OK"
                    );
                    return;
                }

                bool sameEmail = user.Email.Equals(
                    Email.Trim(),
                    StringComparison.OrdinalIgnoreCase
                );

                if (!sameEmail)
                {
                    await DialogService.ShowAlertAsync(
                        "Invalid user",
                        "Biometric login is not enabled for this email.",
                        "OK"
                    );
                    return;
                }

                if (!user.BiometricEnabled)
                {
                    await DialogService.ShowAlertAsync(
                        "Biometric disabled",
                        "Please login with password first and enable biometric login.",
                        "OK"
                    );
                    return;
                }

                bool authenticated = await _biometricService.AuthenticateAsync();

                if (!authenticated)
                {
                    await DialogService.ShowAlertAsync(
                        "Access denied",
                        "Biometric authentication failed.",
                        "OK"
                    );
                    return;
                }

                await DialogService.ShowAlertAsync(
                    "Welcome",
                    $"Hello {user.Name}!",
                    "OK"
                );

                await GoToAppShellAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task GoToRegisterAsync()
        {
            RegisterPage registerPage = _serviceProvider.GetRequiredService<RegisterPage>();
            await NavigationService.PushAsync(registerPage);
        }

        private async Task GoToAppShellAsync()
        {
            AppShell appShell = _serviceProvider.GetRequiredService<AppShell>();
            await NavigationService.SetRootAsync(appShell);
        }

       

        private async Task CheckBiometricAvailabilityAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                IsBiometricButtonVisible = false;
                return;
            }

            UserModel? savedUser = await _sessionService.GetUserSessionAsync();

            if (savedUser == null)
            {
                IsBiometricButtonVisible = false;
                return;
            }

            bool sameEmail = savedUser.Email.Equals(
                Email.Trim(),
                StringComparison.OrdinalIgnoreCase
            );

            IsBiometricButtonVisible = sameEmail && savedUser.BiometricEnabled;
        }

        public bool IsLoginLoading
        {
            get => _isLoginLoading;
            set => SetProperty(ref _isLoginLoading, value);
        }

        public bool IsGoogleLoading
        {
            get => _isGoogleLoading;
            set => SetProperty(ref _isGoogleLoading, value);
        }
    }
}
