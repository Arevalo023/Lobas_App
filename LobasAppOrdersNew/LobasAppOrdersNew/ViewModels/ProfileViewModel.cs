using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly SessionService _sessionService;
        private readonly UserApiService _userApiService;

        private UserModel? _currentUser;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _authProvider = string.Empty;
        private string _biometricStatus = string.Empty;
        private string _lastNameChangedText = "Sin cambios registrados";
        private bool _biometricEnabled;
        private bool _isSaving;

        public ProfileViewModel(
            SessionService sessionService,
            UserApiService userApiService,
            IDialogService dialogService,
            INavigationService navigationService)
            : base(dialogService, navigationService)
        {
            _sessionService = sessionService;
            _userApiService = userApiService;

            Title = "Perfil";

            LoadProfileCommand = new Command(async () => await LoadProfileAsync());
            SaveNameCommand = new Command(async () => await SaveNameAsync());
            DisableBiometricCommand = new Command(async () => await DisableBiometricAsync());
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

        public string AuthProvider
        {
            get => _authProvider;
            set => SetProperty(ref _authProvider, value);
        }

        public bool BiometricEnabled
        {
            get => _biometricEnabled;
            set => SetProperty(ref _biometricEnabled, value);
        }

        public string BiometricStatus
        {
            get => _biometricStatus;
            set => SetProperty(ref _biometricStatus, value);
        }

        public string LastNameChangedText
        {
            get => _lastNameChangedText;
            set => SetProperty(ref _lastNameChangedText, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public ICommand LoadProfileCommand { get; }

        public ICommand SaveNameCommand { get; }

        public ICommand DisableBiometricCommand { get; }

        public async Task LoadProfileAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                UserModel? sessionUser = await _sessionService.GetUserSessionAsync();

                if (sessionUser == null)
                {
                    await DialogService.ShowAlertAsync("Sesion", "No hay una sesion activa.", "OK");
                    return;
                }

                UserModel? serverUser = await _userApiService.GetUserByIdAsync(sessionUser.Id);
                _currentUser = serverUser ?? sessionUser;

                await _sessionService.SaveUserSessionAsync(_currentUser);

                ApplyUser(_currentUser);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveNameAsync()
        {
            if (IsBusy || _currentUser == null)
            {
                return;
            }

            string newName = Name.Trim();

            if (string.IsNullOrWhiteSpace(newName))
            {
                await DialogService.ShowAlertAsync("Validacion", "El nombre es obligatorio.", "OK");
                return;
            }

            if (string.Equals(_currentUser.Name.Trim(), newName, StringComparison.Ordinal))
            {
                await DialogService.ShowAlertAsync(
                    "Sin cambios",
                    "Dejaste el mismo nombre, no hay cambios.",
                    "OK");
                return;
            }

            try
            {
                IsBusy = true;
                IsSaving = true;

                ApiResponse<UserModel> response =
                    await _userApiService.UpdateNameAsync(_currentUser.Id, newName);

                if (response.User == null)
                {
                    await DialogService.ShowAlertAsync("Perfil", response.Message, "OK");
                    return;
                }

                _currentUser = response.User;
                await _sessionService.SaveUserSessionAsync(_currentUser);
                ApplyUser(_currentUser);

                await DialogService.ShowAlertAsync("Perfil", response.Message, "OK");
            }
            finally
            {
                IsSaving = false;
                IsBusy = false;
            }
        }

        private async Task DisableBiometricAsync()
        {
            if (IsBusy || _currentUser == null)
            {
                return;
            }

            if (!_currentUser.BiometricEnabled)
            {
                await DialogService.ShowAlertAsync(
                    "Biometria",
                    "La biometria ya esta deshabilitada.",
                    "OK");
                return;
            }

            bool confirm = await DialogService.ShowConfirmationAsync(
                "Deshabilitar biometria",
                "Seguro que deseas deshabilitar el acceso biometrico?",
                "Si",
                "Cancelar");

            if (!confirm)
            {
                return;
            }

            ApiResponse<UserModel> response =
                await _userApiService.UpdateBiometricStatusAsync(_currentUser.Id, false);

            if (!response.Message.Contains("successfully", StringComparison.OrdinalIgnoreCase))
            {
                await DialogService.ShowAlertAsync("Error", response.Message, "OK");
                return;
            }

            _currentUser.BiometricEnabled = false;
            await _sessionService.SaveUserSessionAsync(_currentUser);
            ApplyUser(_currentUser);

            await DialogService.ShowAlertAsync(
                "Biometria",
                "Biometria deshabilitada correctamente.",
                "OK");
        }

        private void ApplyUser(UserModel user)
        {
            Name = user.Name;
            Email = user.Email;
            AuthProvider = user.AuthProvider;
            BiometricEnabled = user.BiometricEnabled;
            BiometricStatus = user.BiometricEnabled ? "Habilitada" : "Deshabilitada";
            LastNameChangedText = user.LastNameChangedAt.HasValue
                ? $"Ultimo cambio: {user.LastNameChangedAt.Value:dd/MM/yyyy HH:mm}"
                : "Sin cambios registrados";
        }
    }
}
