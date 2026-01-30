using backend.XML;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class ReadController : Controller
    {
        private readonly XMLMapper _xmlMapper;

        public ReadController(XMLMapper xmlMapper)
        {
            _xmlMapper = xmlMapper;
        }

        [HttpGet("Read/Analytics/{code}")]
        public IActionResult GetLinkByShortCode(string code)
        {
            var collection = _xmlMapper.LoadOrCreate();
            var link = collection.Links.FirstOrDefault(l => l.ShortURL == code);

            return Ok(link);
        }
    }
}