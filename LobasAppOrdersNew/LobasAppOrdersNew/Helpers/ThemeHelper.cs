namespace LobasAppOrdersNew.Helpers
{
    public static class ThemeHelper
    {
        public static void ApplyPinkTheme()
        {
            Application.Current!.Resources["AppBackgroundColor"] = Color.FromArgb("#FFF1F2");
            Application.Current.Resources["AppPrimaryColor"] = Color.FromArgb("#DB2777");
            Application.Current.Resources["AppSecondaryColor"] = Color.FromArgb("#BE185D");
            Application.Current.Resources["AppTextColor"] = Color.FromArgb("#831843");
            Application.Current.Resources["AppCardColor"] = Color.FromArgb("#FFFFFF");
            Application.Current.Resources["AppBorderColor"] = Color.FromArgb("#FBCFE8");
            Application.Current.Resources["AppMutedTextColor"] = Color.FromArgb("#6B7280");
            Application.Current.Resources["AppButtonTextColor"] = Color.FromArgb("#FFFFFF");

            Application.Current.Resources["AppTitleFontSize"] = 30.0;
            Application.Current.Resources["AppSubtitleFontSize"] = 14.0;
            Application.Current.Resources["AppBodyFontSize"] = 15.0;
        }

        public static void ApplyDarkTheme()
        {
            Application.Current!.Resources["AppBackgroundColor"] = Color.FromArgb("#111827");
            Application.Current.Resources["AppPrimaryColor"] = Color.FromArgb("#6366F1");
            Application.Current.Resources["AppSecondaryColor"] = Color.FromArgb("#A5B4FC");
            Application.Current.Resources["AppTextColor"] = Color.FromArgb("#F9FAFB");
            Application.Current.Resources["AppCardColor"] = Color.FromArgb("#1F2937");
            Application.Current.Resources["AppBorderColor"] = Color.FromArgb("#374151");
            Application.Current.Resources["AppMutedTextColor"] = Color.FromArgb("#D1D5DB");
            Application.Current.Resources["AppButtonTextColor"] = Color.FromArgb("#FFFFFF");

            Application.Current.Resources["AppTitleFontSize"] = 32.0;
            Application.Current.Resources["AppSubtitleFontSize"] = 15.0;
            Application.Current.Resources["AppBodyFontSize"] = 16.0;
        }
    }
}