using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using LobasAppOrdersNew.Models;

namespace LobasAppOrdersNew.Services
{
    public class GoogleAuthService
    {
        private readonly GoogleAuthSettings _googleAuthSettings;

        public GoogleAuthService(GoogleAuthSettings googleAuthSettings)
        {
            _googleAuthSettings = googleAuthSettings;
        }

        public async Task<GoogleUserInfo?> LoginWithGoogleAsync()
        {
            try
            {
                ClientSecrets secrets = new ClientSecrets
                {
                    ClientId = _googleAuthSettings.ClientId,
                    ClientSecret = _googleAuthSettings.ClientSecret
                };

                string[] scopes =
                {
            Oauth2Service.Scope.UserinfoEmail,
            Oauth2Service.Scope.UserinfoProfile
        };

                var dataStore = new FileDataStore("LobasOrders.GoogleAuth");

                // Esto borra la sesión guardada de Google para que no entre automático
                await dataStore.DeleteAsync<TokenResponse>("user");

                using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(
        TimeSpan.FromSeconds(60)
    );

                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    scopes,
                    "user",
                    cancellationTokenSource.Token,
                    dataStore
                );

                Oauth2Service oauthService = new Oauth2Service(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Lobas Orders"
                    }
                );

                Userinfo userInfo = await oauthService.Userinfo.Get().ExecuteAsync();

                if (userInfo == null)
                {
                    return null;
                }

                return new GoogleUserInfo
                {
                    Id = userInfo.Id ?? string.Empty,
                    Email = userInfo.Email ?? string.Empty,
                    Name = userInfo.Name ?? userInfo.Email ?? "Google User"
                };
            }
            catch (TaskCanceledException)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Google login",
                    "Google sign-in was cancelled. Please try again.",
                    "OK"
                );

                return null;
            }
            catch (OperationCanceledException)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Google login",
                    "Google sign-in was cancelled. Please try again.",
                    "OK"
                );

                return null;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Google login error",
                    $"Could not complete Google sign-in. Please try again.\n\nDetails: {ex.Message}",
                    "OK"
                );

                return null;
            }
        }
    }
}