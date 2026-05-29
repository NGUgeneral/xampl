using xampl.Services.RateLimiterService;

namespace xampl.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, CloudRateLimiterService rateLimiter)
        {
            var userIp = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            var cacheKey = $"xampl:rate:{userIp}";
            bool isAllowed = await rateLimiter.IsRequestAllowedAsync(cacheKey);

            if (!isAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("429 Too Many Requests: Rate limit exceeded. Please try again later.");
                return;
            }

            await _next(context);
        }
    }
}
