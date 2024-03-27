using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.Contracts;
using CamAI.EdgeBox.Services.Utils;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(
    ILogger<TestController> logger,
    IPublishEndpoint bus,
    IMemoryCache cache
) : ControllerBase
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
            GlobalData.Cameras.First(),
            @"C:\Users\Administrator\Downloads\ffmpeg-2024-02-29-git-4a134eb14a-full_build\bin"
        );
        rtsp.CaptureFrame("yes_please");
    }

    [HttpPost("camera/check")]
    public void CheckCameraConnection()
    {
        var cameara = new Camera
        {
            Host = "127.0.0.1",
            Port = 554,
            Username = "admin",
            Password = "Admin123@",
            Path = "Streaming/channels/101",
            Protocol = "rtsp"
        };
        cameara.CheckConnection();
    }

    [HttpPut("camera/sync")]
    public void SyncCamera()
    {
        bus.Publish(CameraChangeMessage.ToUpsertMessage(GlobalData.Cameras[0]));
    }
}
