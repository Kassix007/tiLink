using backend.Models;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GetTextController : ControllerBase
{
    private readonly FileService _file;

    public GetTextController(FileService file)
    {
        _file = file;
    }

    [HttpPost("addtoFile")]
    public IActionResult addToFile([FromBody] Link link)
    {
        _file.AddToFile(link);
        return Ok("Saved to CSV");
    }

    [HttpGet("GetAllLongURLs")]
    public IActionResult getAllLongURLs()
    {
        var urls = _file.GetLongUrlsById();
        return Ok(urls);
    }

    [HttpGet("GetLongUrlById")]
    public IActionResult getLongUrlById(Guid id)
    {
        var url = _file.GetLongUrlById(id);
        return Ok(url);
    }
}
