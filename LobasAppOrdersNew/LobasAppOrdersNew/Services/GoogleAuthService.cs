using Google.Apis.Auth.OAuth2;
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

                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("LobasOrders.GoogleAuth")
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
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Google login error",
                    ex.Message,
                    "OK"
                );

                return null;
            }
        }
    }
}