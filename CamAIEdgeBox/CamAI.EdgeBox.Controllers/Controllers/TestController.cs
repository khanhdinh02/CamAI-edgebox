using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(IPublishEndpoint bus, ILogger<TestController> logger, IMemoryCache cache) : ControllerBase
{
    [HttpGet("{name}")]
    public async Task<ActionResult<string>> TestConnection([FromRoute] string name)
    {
        logger.LogInformation($"CPU usage: {cache.Get("CpuUsage")}");
        return $"Hello {name} from edge box";
    }

    [HttpGet("capture")]
    public void TestCaptureFrame()
    {
        var rtsp = new RtspExtension(
            new Uri("rtsp://admin:Admin123%40@localhost:554/Streaming/channels/101"),
            @"C:\Users\Administrator\Downloads\ffmpeg-2024-02-29-git-4a134eb14a-full_build\bin"
        );
        rtsp.CaptureFrame("yes_please");
    }
}
