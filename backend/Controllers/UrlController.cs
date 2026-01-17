namespace backend.Controllers
{
    using backend.Models;
    using backend.Service;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("")]
    public class UrlController : ControllerBase
    {
        private readonly UrlStore _store;
        private readonly NgrokService _ngrok;

        public UrlController(UrlStore store, NgrokService ngrok)
        {
            _store = store;
            _ngrok = ngrok;
        }

        [HttpPost("shorten")]
        public IActionResult Shorten([FromBody] ShortenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LongUrl))
                return BadRequest("Invalid URL");

            var code = Guid.NewGuid().ToString("N")[..6];
            _store.Save(code, request.LongUrl);

            var baseUrl = _ngrok.PublicUrl ?? $"{Request.Scheme}://{Request.Host}";
            return Ok(new { shortUrl = $"{baseUrl}/{code}" });
        }

        [HttpGet("{code}")]
        public IActionResult RedirectToLongUrl(string code)
        {
            var longUrl = _store.Get(code);

            if (longUrl == null)
                return NotFound("Short URL not found");

            return Redirect(longUrl);
        }
    }

}
