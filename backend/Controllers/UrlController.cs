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
        private readonly XMLMapper _xmlMapper;

        public UrlController(UrlStore store, NgrokService ngrok, XMLMapper xmlMapper)
        {
            _store = store;
            _ngrok = ngrok;
            _xmlMapper = xmlMapper;
        }

        [HttpPost("shorten")]
        public IActionResult Shorten([FromBody] ShortenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LongUrl))
                return BadRequest("Invalid URL");

            var link = _xmlMapper.CreateAndSaveLink(request.LongUrl);
            _store.Save(link.ShortURL, request.LongUrl);

            var baseUrl = _ngrok.PublicUrl ?? $"{Request.Scheme}://{Request.Host}";
            return Ok(new { shortUrl = $"{baseUrl}/{link.ShortURL}" });
        }

        [HttpGet("{code}")]
        public IActionResult RedirectToLongUrl(string code)
        {
            var longUrl = _store.Get(code);

            _xmlMapper.TrackDevice(code);

            if (longUrl == null)
                return NotFound("Short URL not found");

            return Redirect(longUrl);
        }
    }
}
