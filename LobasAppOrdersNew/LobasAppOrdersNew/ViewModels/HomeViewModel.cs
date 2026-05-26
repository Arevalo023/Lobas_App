using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;
using LobasAppOrdersNew.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LobasAppOrdersNew.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly SessionService _sessionService;
        private readonly UserApiService _userApiService;
        private readonly IServiceProvider _serviceProvider;

        private string _welcomeMessage = "Welcome";
        private bool _biometricEnabled;

        public HomeViewModel(
            SessionService sessionService,
            UserApiService userApiService,
            IServiceProvider serviceProvider,
            IDialogService dialogService,
            INavigationService navigationService)
            : base(dialogService, navigationService)
        {
            _sessionService = sessionService;
            _userApiService = userApiService;
            _serviceProvider = serviceProvider;

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
                await DialogService.ShowAlertAsync("Session", "No active session found.", "OK");
                return;
            }

            ApiResponse<UserModel> response =
                await _userApiService.UpdateBiometricStatusAsync(user.Id, enabled);

            if (response.User == null)
            {
                await DialogService.ShowAlertAsync("Error", response.Message, "OK");
                return;
            }

            await _sessionService.SaveUserSessionAsync(response.User);
            BiometricEnabled = response.User.BiometricEnabled;

            string message = enabled
                ? "Biometric login enabled."
                : "Biometric login disabled.";

            await DialogService.ShowAlertAsync("Success", message, "OK");
        }

        private async Task LogoutAsync()
        {
            UserModel? user = await _sessionService.GetUserSessionAsync();

            if (user == null)
            {
                _sessionService.ClearSession();
                await GoToLoginAsync();
                return;
            }

            if (!user.BiometricEnabled)
            {
                _sessionService.ClearSession();
            }

            await GoToLoginAsync();
        }

        private async Task GoToLoginAsync()
        {
            LoginPage loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            await NavigationService.SetRootAsync(new NavigationPage(loginPage));
        }
    }
}
