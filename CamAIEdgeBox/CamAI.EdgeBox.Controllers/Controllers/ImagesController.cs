using System.Net.Mime;
using CamAI.EdgeBox.Services.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImagesController(IOptions<AiConfiguration> configuration) : Controller
{
    private readonly string basePath = configuration.Value.EvidenceOutputDir;

    [HttpGet]
    public FileStreamResult GetFile([FromQuery(Name = "path")] string filePath)
    {
        var file = Path.Combine(basePath, filePath);
        var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        return File(fileStream, MediaTypeNames.Image.Png);
    }
}
