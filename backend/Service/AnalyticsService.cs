using UAParser;

namespace backend.Service
{
    public class AnalyticsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnalyticsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIp()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
                return "Unknown";

            string? ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = ipAddress.Split(',').First().Trim();
                return ipAddress;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
//Find alternative for ngrok path in appsettings.json so that it does not conflict with other machines