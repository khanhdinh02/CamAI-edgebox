using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
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

    [HttpGet("{id}/stream")]
    public IActionResult GetM3U8File([FromQuery] Guid id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return File(
            cameraService.GetM3U8File(id),
            "application/octet-stream",
            enableRangeProcessing: true
        );
    }

    [HttpGet("{id}/stream/{tsFileName}")]
    public IActionResult GetTsFile([FromQuery] Guid id, [FromQuery] string tsFileName)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return File(
            cameraService.GetTsFile(id, tsFileName),
            "application/octet-stream",
            enableRangeProcessing: true
        );
    }
}
