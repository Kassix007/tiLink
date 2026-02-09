using backend.XML;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class ReadController : Controller
    {
        private readonly XMLService _xmlService;

        public ReadController(XMLService xmlService)
        {
            _xmlService = xmlService;
        }

        [HttpGet("Read/Analytics/{code}")]
        public IActionResult GetLinkByShortCode(string code)
        {
            var collection = _xmlService.LoadOrCreate();
            var link = collection.Links.FirstOrDefault(l => l.ShortURL == code);

            return Ok(link);
        }
    }
}