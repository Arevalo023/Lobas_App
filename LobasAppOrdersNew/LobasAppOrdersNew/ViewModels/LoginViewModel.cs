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

        private string _email = string.Empty;
        private string _password = string.Empty;

        public LoginViewModel(
            AuthApiService authApiService,
            SessionService sessionService,
            AppBiometricService biometricService)
        {
            _authApiService = authApiService;
            _sessionService = sessionService;
            _biometricService = biometricService;

            Title = "Login";

            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () => await GoToRegisterAsync());
            BiometricLoginCommand = new Command(async () => await BiometricLoginAsync());
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        public ICommand BiometricLoginCommand { get; }

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

        private async Task BiometricLoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

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

                if (!user.BiometricEnabled)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Biometric disabled",
                        "Please enable biometric login first.",
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
    }
}