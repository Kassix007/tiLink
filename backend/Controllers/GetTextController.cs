using backend.Models;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GetTextController(FileService file, FileManipulation manipulation) : ControllerBase
{
    private readonly FileService _file = file;
    private readonly FileManipulation _manipulation = manipulation;

    [HttpPost("addtoFile")]
    public IActionResult addToFile([FromBody] Link link)
    {
        _file.AddToFile(link);

        var id = Guid.Parse("b7e1a9c4-5f6d-4b1c-9a23-8e6f2d0c4b11");
        _manipulation.GetLongUrlById(id);

        return Ok("Saved to CSV");
    }
}
