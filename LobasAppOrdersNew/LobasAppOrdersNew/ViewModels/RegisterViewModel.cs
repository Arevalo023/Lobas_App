using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthApiService _authApiService;

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        public RegisterViewModel(
            AuthApiService authApiService,
            IDialogService dialogService,
            INavigationService navigationService)
            : base(dialogService, navigationService)
        {
            _authApiService = authApiService;
            Title = "Register";

            RegisterCommand = new Command(async () => await RegisterAsync());
            GoToLoginCommand = new Command(async () => await GoToLoginAsync());
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand RegisterCommand { get; }

        public ICommand GoToLoginCommand { get; }

        private async Task RegisterAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                await DialogService.ShowAlertAsync("Validation", "Name is required.", "OK");
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

            if (Password != ConfirmPassword)
            {
                await DialogService.ShowAlertAsync("Validation", "Passwords do not match.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                RegisterRequest request = new RegisterRequest
                {
                    Name = Name.Trim(),
                    Email = Email.Trim(),
                    Password = Password
                };

                ApiResponse<UserModel>? response = await _authApiService.RegisterAsync(request);

                if (response == null)
                {
                    await DialogService.ShowAlertAsync("Error", "No response from server.", "OK");
                    return;
                }

                if (response.User == null)
                {
                    await DialogService.ShowAlertAsync("Register failed", response.Message, "OK");
                    return;
                }

                await DialogService.ShowAlertAsync(
                    "Success",
                    "Account created successfully. Please log in.",
                    "OK"
                );

                await NavigationService.GoBackAsync();
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

        private async Task GoToLoginAsync()
        {
            await NavigationService.GoBackAsync();
        }
    }
}
