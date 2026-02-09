namespace backend.Controllers
{
    using backend.Models;
    using backend.Models.Analytics;
    using backend.Service;
    using backend.XML;
    using Microsoft.AspNetCore.Mvc;
    using static backend.XML.XMLModel;

    [ApiController]
    [Route("")]
    public class UrlController : ControllerBase
    {
        private readonly UrlStore _store;
        private readonly NgrokService _ngrok;
        private readonly XMLService _xmlService;
        private readonly ILogger<UrlController> _logger;

        public UrlController(UrlStore store, NgrokService ngrok, XMLService xmlService, ILogger<UrlController> logger)
        {
            _store = store;
            _ngrok = ngrok;
            _xmlService = xmlService;
            _logger = logger;
        }

        [HttpPost("shorten")]
        public IActionResult Shorten([FromBody] ShortenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LongUrl))
            {
                _logger.LogError("URL provided was not valid.");
                return BadRequest("Invalid URL");
            }

            var link = _xmlService.CreateAndSaveLink(request.LongUrl);
            _store.Save(link.ShortURL, request.LongUrl);

            var baseUrl = _ngrok.PublicUrl ?? $"{Request.Scheme}://{Request.Host}";
            return Ok(new { shortUrl = $"{baseUrl}/{link.ShortURL}" });
        }

        [HttpGet("{code}")]
        public IActionResult RedirectToLongUrl(string code)
        {
            var longUrl = _store.Get(code);

            _xmlService.TrackDevice(code);

            if (longUrl == null)
            {
                _logger.LogError("Short URL not found");
                return NotFound("Short URL not found");
            }
            return Redirect(longUrl);
        }
    }
}
