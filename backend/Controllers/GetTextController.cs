using backend.Models;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GetTextController(FileService file) : ControllerBase
{
    private readonly FileService _file = file;

    [HttpPost("addtoFile")]
    public IActionResult addToFile([FromBody] Link link)
    {
        _file.addToFile(link);
        return Ok("Saved to CSV");
    }
}
