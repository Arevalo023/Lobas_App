using Plugin.Maui.Biometric;

using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.Services
{
    public class AppBiometricService
    {
        private readonly IBiometric _biometric;
        private readonly IDialogService _dialogService;

        public AppBiometricService(IBiometric biometric, IDialogService dialogService)
        {
            _biometric = biometric;
            _dialogService = dialogService;
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                var result = await _biometric.AuthenticateAsync(
                    new AuthenticationRequest
                    {
                        Title = "Lobas Orders",
                        Subtitle = "Use your fingerprint or Windows Hello to login",
                        NegativeText = "Cancel"
                    },
                    CancellationToken.None
                );

                return result.Status == BiometricResponseStatus.Success;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Biometric error",
                    ex.Message,
                    "OK"
                );

                return false;
            }
        }
    }
}