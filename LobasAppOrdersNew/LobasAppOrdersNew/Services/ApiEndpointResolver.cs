using Microsoft.Extensions.Configuration;

namespace LobasAppOrdersNew.Services
{
    public static class ApiEndpointResolver
    {
        public static string GetBaseUrl(IConfiguration configuration)
        {
            string? baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? configuration["ApiSettings:AndroidBaseUrl"]
                : configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = configuration["ApiSettings:BaseUrl"];
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");
            }

            return baseUrl.EndsWith("/", StringComparison.Ordinal)
                ? baseUrl
                : $"{baseUrl}/";
        }

        public static string GetNotificationsHubUrl(IConfiguration configuration)
        {
            Uri apiBaseUri = new Uri(GetBaseUrl(configuration));
            Uri rootUri = new Uri(apiBaseUri.GetLeftPart(UriPartial.Authority));

            return new Uri(rootUri, "hubs/notifications").ToString();
        }
    }
}
