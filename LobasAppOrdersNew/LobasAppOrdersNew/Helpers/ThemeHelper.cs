namespace LobasAppOrdersNew.Helpers
{
    public static class ThemeHelper
    {
        public const string PinkThemeKey = "pink";
        public const string DarkThemeKey = "dark";

        private const string SelectedThemePreferenceKey = "SelectedTheme";

        public static event EventHandler? ThemeChanged;

        public static string CurrentThemeKey =>
            Preferences.Default.Get(SelectedThemePreferenceKey, PinkThemeKey);

        public static void ApplySavedTheme()
        {
            ApplyTheme(CurrentThemeKey, savePreference: false);
        }

        public static void ApplyTheme(string themeKey)
        {
            ApplyTheme(themeKey, savePreference: true);
        }

        public static void ApplyPinkTheme()
        {
            ApplyTheme(PinkThemeKey);
        }

        public static void ApplyDarkTheme()
        {
            ApplyTheme(DarkThemeKey);
        }

        public static string GetThemeDisplayName(string themeKey)
        {
            return themeKey == DarkThemeKey
                ? "Nocturno profesional"
                : "Lobas claro";
        }

        private static void ApplyTheme(string themeKey, bool savePreference)
        {
            if (themeKey == DarkThemeKey)
            {
                ApplyDarkResources();
            }
            else
            {
                ApplyPinkResources();
                themeKey = PinkThemeKey;
            }

            if (savePreference)
            {
                Preferences.Default.Set(SelectedThemePreferenceKey, themeKey);
            }

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void ApplyPinkResources()
        {
            Application.Current!.Resources["AppBackgroundColor"] = Color.FromArgb("#FFF1F2");
            Application.Current.Resources["AppPrimaryColor"] = Color.FromArgb("#DB2777");
            Application.Current.Resources["AppSecondaryColor"] = Color.FromArgb("#BE185D");
            Application.Current.Resources["AppTextColor"] = Color.FromArgb("#831843");
            Application.Current.Resources["AppCardColor"] = Color.FromArgb("#FFFFFF");
            Application.Current.Resources["AppBorderColor"] = Color.FromArgb("#FBCFE8");
            Application.Current.Resources["AppMutedTextColor"] = Color.FromArgb("#6B7280");
            Application.Current.Resources["AppButtonTextColor"] = Color.FromArgb("#FFFFFF");
            Application.Current.Resources["AppInputBackgroundColor"] = Color.FromArgb("#FFFFFF");
            Application.Current.Resources["AppPlaceholderColor"] = Color.FromArgb("#C08497");
            Application.Current.Resources["AppDangerColor"] = Color.FromArgb("#DC2626");
            Application.Current.Resources["AppEditColor"] = Color.FromArgb("#6366F1");
            Application.Current.Resources["AppShellHeaderColor"] = Color.FromArgb("#831843");
            Application.Current.Resources["AppShellHeaderTextColor"] = Color.FromArgb("#FFFFFF");
            Application.Current.Resources["AppShellHeaderMutedTextColor"] = Color.FromArgb("#FCE7F3");

            Application.Current.Resources["AppBodyFontFamily"] = "OpenSansRegular";
            Application.Current.Resources["AppTitleFontFamily"] = "OpenSansSemibold";
            Application.Current.Resources["AppButtonFontFamily"] = "OpenSansSemibold";
            Application.Current.Resources["HomeIconSource"] = "home.png";
            Application.Current.Resources["CustomersIconSource"] = "customers.png";
            Application.Current.Resources["ProductsIconSource"] = "product.png";
            Application.Current.Resources["OrdersIconSource"] = "orders.png";
            Application.Current.Resources["ProfileIconSource"] = "settings.png";
            Application.Current.Resources["SettingsIconSource"] = "settings.png";
            Application.Current.Resources["AboutIconSource"] = "about.png";
            Application.Current.Resources["LogoutIconSource"] = "logout.png";
            Application.Current.Resources["AppTitleFontSize"] = 30.0;
            Application.Current.Resources["AppSubtitleFontSize"] = 14.0;
            Application.Current.Resources["AppBodyFontSize"] = 15.0;
            Application.Current.Resources["AppCaptionFontSize"] = 13.0;
        }

        private static void ApplyDarkResources()
        {
            Application.Current!.Resources["AppBackgroundColor"] = Color.FromArgb("#111827");
            Application.Current.Resources["AppPrimaryColor"] = Color.FromArgb("#22C55E");
            Application.Current.Resources["AppSecondaryColor"] = Color.FromArgb("#86EFAC");
            Application.Current.Resources["AppTextColor"] = Color.FromArgb("#F9FAFB");
            Application.Current.Resources["AppCardColor"] = Color.FromArgb("#1F2937");
            Application.Current.Resources["AppBorderColor"] = Color.FromArgb("#374151");
            Application.Current.Resources["AppMutedTextColor"] = Color.FromArgb("#CBD5E1");
            Application.Current.Resources["AppButtonTextColor"] = Color.FromArgb("#052E16");
            Application.Current.Resources["AppInputBackgroundColor"] = Color.FromArgb("#0F172A");
            Application.Current.Resources["AppPlaceholderColor"] = Color.FromArgb("#94A3B8");
            Application.Current.Resources["AppDangerColor"] = Color.FromArgb("#F87171");
            Application.Current.Resources["AppEditColor"] = Color.FromArgb("#38BDF8");
            Application.Current.Resources["AppShellHeaderColor"] = Color.FromArgb("#0F172A");
            Application.Current.Resources["AppShellHeaderTextColor"] = Color.FromArgb("#F9FAFB");
            Application.Current.Resources["AppShellHeaderMutedTextColor"] = Color.FromArgb("#CBD5E1");

            Application.Current.Resources["AppBodyFontFamily"] = "OpenSansSemibold";
            Application.Current.Resources["AppTitleFontFamily"] = "OpenSansSemibold";
            Application.Current.Resources["AppButtonFontFamily"] = "OpenSansSemibold";
            Application.Current.Resources["HomeIconSource"] = "home_blanco.png";
            Application.Current.Resources["CustomersIconSource"] = "customers_blanco.png";
            Application.Current.Resources["ProductsIconSource"] = "product_blanco.png";
            Application.Current.Resources["OrdersIconSource"] = "orders_blanco.png";
            Application.Current.Resources["ProfileIconSource"] = "settings_blanco.png";
            Application.Current.Resources["SettingsIconSource"] = "settings_blanco.png";
            Application.Current.Resources["AboutIconSource"] = "about_blanco.png";
            Application.Current.Resources["LogoutIconSource"] = "logout_rojo.png";
            Application.Current.Resources["AppTitleFontSize"] = 32.0;
            Application.Current.Resources["AppSubtitleFontSize"] = 15.0;
            Application.Current.Resources["AppBodyFontSize"] = 16.0;
            Application.Current.Resources["AppCaptionFontSize"] = 14.0;
        }
    }
}
