using Microsoft.Extensions.Options;
using System.Net;
using xampl.Services.ConfigOptionsService;

namespace xampl.Services.RateLimitService
{
    public class CloudRateLimiterService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CloudRateLimiterService> _logger;
        private readonly ConfigOptions _config;


        public CloudRateLimiterService(
            HttpClient httpClient,
            ILogger<CloudRateLimiterService> logger,
            IOptions<ConfigOptions> configOptions
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = configOptions.Value;
        }

        public async Task<bool> IsRequestAllowedAsync(string key, int limit = 100, int window = 60)
        {
            try
            {
                var nonce = Guid.NewGuid().ToString("N").Substring(0, 6);

                var url = $"{_config.RateLimiterUrl}/is_allowed?key={Uri.EscapeDataString(key)}&limit={limit}&window={window}&nonce={nonce}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning($"Rate limit breached on AWS for key: {key}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRITICAL: AWS Rate Limiter unreachable. Failing open.");
                return true;
            }
        }
    }
}
