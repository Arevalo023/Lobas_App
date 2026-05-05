using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;

namespace LobasAppOrdersNew.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly SessionService _sessionService;
        private readonly UserApiService _userApiService;

        private string _welcomeMessage = "Welcome";
        private bool _biometricEnabled;

        public HomeViewModel(SessionService sessionService, UserApiService userApiService)
        {
            _sessionService = sessionService;
            _userApiService = userApiService;

            Title = "Home";

            LoadUserCommand = new Command(async () => await LoadUserAsync());
            EnableBiometricCommand = new Command(async () => await UpdateBiometricAsync(true));
            DisableBiometricCommand = new Command(async () => await UpdateBiometricAsync(false));
            LogoutCommand = new Command(async () => await LogoutAsync());
        }

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public bool BiometricEnabled
        {
            get => _biometricEnabled;
            set => SetProperty(ref _biometricEnabled, value);
        }

        public ICommand LoadUserCommand { get; }

        public ICommand EnableBiometricCommand { get; }

        public ICommand DisableBiometricCommand { get; }

        public ICommand LogoutCommand { get; }

        private async Task LoadUserAsync()
        {
            UserModel? user = await _sessionService.GetUserSessionAsync();

            if (user == null)
            {
                WelcomeMessage = "Welcome";
                BiometricEnabled = false;
                return;
            }

            WelcomeMessage = $"Welcome, {user.Name}";
            BiometricEnabled = user.BiometricEnabled;
        }

        private async Task UpdateBiometricAsync(bool enabled)
        {
            UserModel? user = await _sessionService.GetUserSessionAsync();

            if (user == null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Session", "No active session found.", "OK");
                return;
            }

            ApiResponse<UserModel> response =
                await _userApiService.UpdateBiometricStatusAsync(user.Id, enabled);

            if (!response.Message.Contains("successfully", StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", response.Message, "OK");
                return;
            }

            user.BiometricEnabled = enabled;
            await _sessionService.SaveUserSessionAsync(user);
            BiometricEnabled = enabled;

            string message = enabled
                ? "Biometric login enabled."
                : "Biometric login disabled.";

            await Application.Current!.MainPage!.DisplayAlert("Success", message, "OK");
        }

        private async Task LogoutAsync()
        {
            UserModel? user = await _sessionService.GetUserSessionAsync();

            if (user == null)
            {
                _sessionService.ClearSession();
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            if (!user.BiometricEnabled)
            {
                _sessionService.ClearSession();
            }

            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}