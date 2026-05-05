using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;

namespace LobasAppOrdersNew.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthApiService _authApiService;
        private readonly SessionService _sessionService;
        private readonly AppBiometricService _biometricService;
        private readonly GoogleAuthService _googleAuthService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isBiometricButtonVisible;

        public LoginViewModel(
        AuthApiService authApiService,
        SessionService sessionService,
        AppBiometricService biometricService,
        GoogleAuthService googleAuthService)
        {
            _authApiService = authApiService;
            _sessionService = sessionService;
            _biometricService = biometricService;
            _googleAuthService = googleAuthService;

            Title = "Login";

            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () => await GoToRegisterAsync());
            BiometricLoginCommand = new Command(async () => await BiometricLoginAsync());
            GoogleLoginCommand = new Command(async () => await GoogleLoginAsync());
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



        private async Task LoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current!.MainPage!.DisplayAlert("Validation", "Email is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current!.MainPage!.DisplayAlert("Validation", "Password is required.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                LoginRequest request = new LoginRequest
                {
                    Email = Email.Trim(),
                    Password = Password
                };

                ApiResponse<UserModel>? response = await _authApiService.LoginAsync(request);

                if (response == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "No response from server.", "OK");
                    return;
                }

                if (response.User == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Login failed", response.Message, "OK");
                    return;
                }

                await _sessionService.SaveUserSessionAsync(response.User);

                await Application.Current!.MainPage!.DisplayAlert(
                    "Welcome",
                    $"Hello {response.User.Name}!",
                    "OK"
                );

                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
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

                GoogleUserInfo? googleUser = await _googleAuthService.LoginWithGoogleAsync();

                if (googleUser == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Google login",
                        "Google login was cancelled or failed.",
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
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "No response from server.",
                        "OK"
                    );
                    return;
                }

                if (response.User == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Google login failed",
                        response.Message,
                        "OK"
                    );
                    return;
                }

                await _sessionService.SaveUserSessionAsync(response.User);

                await Application.Current!.MainPage!.DisplayAlert(
                    "Welcome",
                    $"Hello {response.User.Name}!",
                    "OK"
                );

                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    ex.Message,
                    "OK"
                );
            }
            finally
            {
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
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Validation",
                        "Please enter your email first.",
                        "OK"
                    );
                    return;
                }

                UserModel? user = await _sessionService.GetUserSessionAsync();

                if (user == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
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
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Invalid user",
                        "Biometric login is not enabled for this email.",
                        "OK"
                    );
                    return;
                }

                if (!user.BiometricEnabled)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Biometric disabled",
                        "Please login with password first and enable biometric login.",
                        "OK"
                    );
                    return;
                }

                bool authenticated = await _biometricService.AuthenticateAsync();

                if (!authenticated)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Access denied",
                        "Biometric authentication failed.",
                        "OK"
                    );
                    return;
                }

                await Application.Current!.MainPage!.DisplayAlert(
                    "Welcome",
                    $"Hello {user.Name}!",
                    "OK"
                );

                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("RegisterPage");
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
    }
}