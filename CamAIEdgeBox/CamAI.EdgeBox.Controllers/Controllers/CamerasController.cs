using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CamerasController(CameraService cameraService, ILogger<CamerasController> logger)
    : Controller
{
    [HttpGet]
    public List<Camera> GetCameras()
    {
        return cameraService.GetCamera();
    }

    [HttpGet("{id}")]
    public Camera GetCamera([FromQuery] Guid id)
    {
        return cameraService.GetCamera(id);
    }

    [HttpPost]
    public Camera AddCamera([FromBody] Camera camera)
    {
        return cameraService.AddCamera(camera);
    }

    [HttpPut("{id}")]
    public Camera UpdateCamera([FromQuery] Guid id, [FromBody] Camera camera)
    {
        return cameraService.UpdateCamera(id, camera);
    }

    [HttpDelete("{id}")]
    public void DeleteCamera([FromQuery] Guid id)
    {
        cameraService.DeleteCamera(id);
    }

    [HttpGet("Example")]
    public async Task<IActionResult> Example()
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return File(
            System.IO.File.OpenRead(
                @"C:\Users\ngud\Downloads\ffmpeg-6.1.1-essentials_build\bin\example.m3u8"
            ),
            "application/octet-stream",
            enableRangeProcessing: true
        );
    }

    [HttpGet("example/{tsFileName}")]
    public async Task<IActionResult> Example_GetTS(string tsFileName)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return File(
            System.IO.File.OpenRead(
                @"C:\Users\ngud\Downloads\ffmpeg-6.1.1-essentials_build\bin\" + tsFileName
            ),
            "application/octet-stream",
            enableRangeProcessing: true
        );
    }
}
