using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(IPublishEndpoint bus) : ControllerBase
{
    [HttpGet("{name}")]
    public ActionResult<string> TestConnection([FromRoute] string name)
    {
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
