using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using FFMpegCore;
using FFMpegCore.Pipes;
using Microsoft.AspNetCore.Mvc;

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

    // [Route("ws")]
    // [ApiExplorerSettings(IgnoreApi = true)]
    // public async Task Stream()
    // {
    //     if (HttpContext.WebSockets.IsWebSocketRequest)
    //     {
    //         using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
    //         await FFMpegArguments.FromUrlInput(
    //             new Uri("rtsp://admin:Admin123%40@192.168.1.200:554/Streaming/channels/201"))
    //             .OutputToPipe(new StreamPipeSink(webSocket));
    //         await SendTime(webSocket);
    //     }
    //     else
    //     {
    //         HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    //     }
    // }
    //
    // private async Task SendTime(WebSocket webSocket)
    // {
    //     while (true)
    //     {
    //         var bytes = Encoding.UTF8.GetBytes($"Time: {DateTime.Now}");
    //         var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
    //         if (webSocket.State == WebSocketState.Open)
    //             await webSocket.SendAsync(
    //                 arraySegment,
    //                 WebSocketMessageType.Text,
    //                 true,
    //                 CancellationToken.None
    //             );
    //         else if (webSocket.State is WebSocketState.Closed or WebSocketState.Aborted)
    //             break;
    //     }
    // }
}
