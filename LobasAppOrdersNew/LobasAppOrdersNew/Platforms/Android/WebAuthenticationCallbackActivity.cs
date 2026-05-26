using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace LobasAppOrdersNew
{
    internal static class GoogleAuthCallback
    {
        public const string Scheme =
            "com.googleusercontent.apps.687024762996-5ros46fu0mmbaj81ak0h2g12u9kejarg";
    }

    [Activity(NoHistory = true, Exported = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = GoogleAuthCallback.Scheme,
        DataPathPrefix = "/oauth2redirect")]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
}