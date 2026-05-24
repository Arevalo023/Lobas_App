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
    }
}
