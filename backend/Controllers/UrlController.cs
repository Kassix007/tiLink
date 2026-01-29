namespace backend.Controllers
{
    using backend.Models;
    using backend.Models.Analytics;
    using backend.Service;
    using backend.XML;
    using Microsoft.AspNetCore.Mvc;
    using UAParser;
    using static backend.XML.XMLModel;

    [ApiController]
    [Route("")]
    public class UrlController : ControllerBase
    {
        private readonly UrlStore _store;
        private readonly NgrokService _ngrok;
        private readonly AnalyticsService _analyticsService;
        private readonly DeviceService _deviceService;
        private readonly FileService _file;
        private readonly XMLMapper _xmlMapper;

        public UrlController(UrlStore store, NgrokService ngrok, AnalyticsService analyticsService, DeviceService deviceService, FileService file, XMLMapper xmlMapper)
        {
            _store = store;
            _ngrok = ngrok;
            _analyticsService = analyticsService;
            _deviceService = deviceService;
            _file = file;
            _xmlMapper = xmlMapper;
        }

        [HttpPost("shorten")]
        public IActionResult Shorten([FromBody] ShortenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LongUrl))
                return BadRequest("Invalid URL");

            var code = Guid.NewGuid().ToString("N")[..6];

            var link = new Link
            {
                Id = Guid.NewGuid(),
                LongURL = request.LongUrl,
                ShortURL = code,
                ExpiryDate = "2050"
            };

            _xmlMapper.SaveOrAppend(link);

            _store.Save(code, request.LongUrl);

            var baseUrl = _ngrok.PublicUrl ?? $"{Request.Scheme}://{Request.Host}";
            return Ok(new { shortUrl = $"{baseUrl}/{code}" });
        }

        [HttpGet("{code}")]
        public IActionResult RedirectToLongUrl(string code)
        {
            var longUrl = _store.Get(code);

            string ipAddress = _analyticsService.GetClientIp();

            DeviceInfo info = _deviceService.GetDeviceInfo();

            var collection = _xmlMapper.LoadOrCreate();
            var link = collection.Links.FirstOrDefault(x => x.ShortURL == code) ?? new LinkAnalyticsXml();

            _xmlMapper.AppendDevice(link, info);

            _file.AddDeviceInfoToFile(info, code);

            if (longUrl == null)
                return NotFound("Short URL not found");

            return Redirect(longUrl);
        }
    }
}
