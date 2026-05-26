using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services.Interfaces;
using Microsoft.Maui.Authentication;

namespace LobasAppOrdersNew.Services
{
    public class GoogleAuthService
    {
#if ANDROID
private string GetAndroidRedirectUri()
{
    string androidClientId = _googleAuthSettings.AndroidClientId;

    string androidScheme = "com.googleusercontent.apps." +
        androidClientId.Replace(".apps.googleusercontent.com", "");

    return $"{androidScheme}:/oauth2redirect";
}
#endif

        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly IDialogService _dialogService;

        public GoogleAuthService(GoogleAuthSettings googleAuthSettings, IDialogService dialogService)
        {
            _googleAuthSettings = googleAuthSettings;
            _dialogService = dialogService;
        }

        public async Task<GoogleUserInfo?> LoginWithGoogleAsync()
        {
#if ANDROID
            return await LoginWithGoogleOnAndroidAsync();
#else
            return await LoginWithGoogleDesktopAsync();
#endif
        }

#if ANDROID
        private async Task<GoogleUserInfo?> LoginWithGoogleOnAndroidAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_googleAuthSettings.AndroidClientId))
                {
                    await _dialogService.ShowAlertAsync(
                        "Google login setup",
                        "GoogleAuth:AndroidClientId is missing. Create an Android or native OAuth client in Google Cloud Console and add it to appsettings.json.",
                        "OK"
                    );

                    return null;
                }

                string codeVerifier = CreateCodeVerifier();
                string codeChallenge = CreateCodeChallenge(codeVerifier);
                string scope = Uri.EscapeDataString("openid email profile");
                string clientId = Uri.EscapeDataString(_googleAuthSettings.AndroidClientId);
               string androidRedirectUri = GetAndroidRedirectUri();
string redirectUri = Uri.EscapeDataString(androidRedirectUri);
                Uri authUri = new Uri(
                    $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope={scope}&code_challenge={codeChallenge}&code_challenge_method=S256&prompt=select_account"
                );

             WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
    authUri,
    new Uri(androidRedirectUri)
);

                if (!authResult.Properties.TryGetValue("code", out string? authorizationCode) ||
                    string.IsNullOrWhiteSpace(authorizationCode))
                {
                    await _dialogService.ShowAlertAsync(
                        "Google login",
                        "Google did not return an authorization code.",
                        "OK"
                    );

                    return null;
                }

                GoogleTokenResponse? tokenResponse = await ExchangeCodeForTokenAsync(
                    authorizationCode,
                    codeVerifier
                );

                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    await _dialogService.ShowAlertAsync(
                        "Google login",
                        "Could not exchange the Google authorization code for an access token.",
                        "OK"
                    );

                    return null;
                }

                return await GetGoogleUserInfoAsync(tokenResponse.AccessToken);
            }
            catch (TaskCanceledException)
            {
                await _dialogService.ShowAlertAsync(
                    "Google login",
                    "Google sign-in was cancelled. Please try again.",
                    "OK"
                );

                return null;
            }
            catch (OperationCanceledException)
            {
                await _dialogService.ShowAlertAsync(
                    "Google login",
                    "Google sign-in was cancelled. Please try again.",
                    "OK"
                );

                return null;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Google login error",
                    $"Could not complete Google sign-in on Android. Please try again.\n\nDetails: {ex.Message}",
                    "OK"
                );

                return null;
            }
        }

        private async Task<GoogleTokenResponse?> ExchangeCodeForTokenAsync(string authorizationCode, string codeVerifier)
        {
            using HttpClient httpClient = new HttpClient();

            Dictionary<string, string> postData = new Dictionary<string, string>
            {
                ["client_id"] = _googleAuthSettings.AndroidClientId,
                ["code"] = authorizationCode,
                ["code_verifier"] = codeVerifier,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = GetAndroidRedirectUri()
            };

            using FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
            using HttpResponseMessage response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using Stream responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<GoogleTokenResponse>(
                responseStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        private async Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string accessToken)
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using HttpResponseMessage response = await httpClient.GetAsync(
                "https://www.googleapis.com/oauth2/v2/userinfo"
            );

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using Stream responseStream = await response.Content.ReadAsStreamAsync();
            GoogleUserInfoResponse? userInfo = await JsonSerializer.DeserializeAsync<GoogleUserInfoResponse>(
                responseStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

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

        private static string CreateCodeVerifier()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return Base64UrlEncode(bytes);
        }

        private static string CreateCodeChallenge(string codeVerifier)
        {
            byte[] bytes = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
            return Base64UrlEncode(bytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
#else
        private async Task<GoogleUserInfo?> LoginWithGoogleDesktopAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_googleAuthSettings.ClientSecret))
                {
                    await _dialogService.ShowAlertAsync(
                        "Google login setup",
                        "Google ClientSecret is missing. Add it to your local ignored appsettings.json under GoogleAuth:ClientSecret or set LOBAS_GOOGLE_CLIENT_SECRET.",
                        "OK"
                    );

                    return null;
                }

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
                await _dialogService.ShowAlertAsync(
                    "Google login",
                    "Google sign-in was cancelled. Please try again.",
                    "OK"
                );

                return null;
            }
            catch (OperationCanceledException)
            {
                await _dialogService.ShowAlertAsync(
                    "Google login",
                    "Google sign-in was cancelled. Please try again.",
                    "OK"
                );

                return null;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync(
                    "Google login error",
                    $"Could not complete Google sign-in. Please try again.\n\nDetails: {ex.Message}",
                    "OK"
                );

                return null;
            }
        }
#endif

        private sealed class GoogleTokenResponse
        {
            public string AccessToken { get; set; } = string.Empty;
        }

        private sealed class GoogleUserInfoResponse
        {
            public string? Id { get; set; }

            public string? Email { get; set; }

            public string? Name { get; set; }
        }
    }
}
