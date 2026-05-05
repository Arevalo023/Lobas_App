using Plugin.Maui.Biometric;

namespace LobasAppOrdersNew.Services
{
    public class AppBiometricService
    {
        private readonly IBiometric _biometric;

        public AppBiometricService(IBiometric biometric)
        {
            _biometric = biometric;
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
                await Application.Current!.MainPage!.DisplayAlert(
                    "Biometric error",
                    ex.Message,
                    "OK"
                );

                return false;
            }
        }
    }
}