using backend.Models.Analytics;
using UAParser;

namespace backend.Service
{
    public class DeviceService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Parser _uaParser;

        public DeviceService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _uaParser = Parser.GetDefault();
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

        public DeviceInfo GetDeviceInfo()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                return new DeviceInfo();

            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var clientInfo = _uaParser.Parse(userAgent);
            var ip = GetClientIp();

            return new DeviceInfo
            {
                IPAddress = ip,
                UserAgent = userAgent,
                Browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}",
                OperatingSystem = $"{clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}",
                DeviceType = GetDeviceType(clientInfo),
                AccessedTime = DateTime.UtcNow
            };
        }

        private string GetDeviceType(ClientInfo clientInfo)
        {
            if (clientInfo.Device.Family.ToLower().Contains("mobile"))
                return "Mobile";

            if (clientInfo.Device.Family.ToLower().Contains("tablet"))
                return "Tablet";

            return "Desktop";
        }
    }
}
