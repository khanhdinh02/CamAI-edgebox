using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CamerasController(
    CameraService cameraService,
    AIService aiService,
    ILogger<CamerasController> logger
) : Controller
{
    [HttpGet]
    public List<Camera> GetCameras()
    {
        return GlobalData.Cameras;
    }

    [HttpGet("{id}")]
    public Camera GetCamera([FromRoute] Guid id)
    {
        return GlobalData.Cameras.Find(x => x.Id == id) ?? throw new Exception("Camera not found");
    }

    [HttpPost]
    public Camera AddCamera([FromBody] Camera cameraDto)
    {
        var camera = cameraService.AddCamera(cameraDto);
        GlobalData.Cameras = cameraService.GetCamera();
        return camera;
    }

    [HttpPut("{id}")]
    public Camera UpdateCamera([FromRoute] Guid id, [FromBody] Camera cameraDto)
    {
        var camera = cameraService.UpdateCamera(id, cameraDto);
        GlobalData.Cameras = cameraService.GetCamera();
        return camera;
    }

    [HttpDelete("{id}")]
    public void DeleteCamera([FromRoute] Guid id)
    {
        cameraService.DeleteCamera(id);
        GlobalData.Cameras = cameraService.GetCamera();
    }

    [HttpGet("{id}/stream/start")]
    public IActionResult GetM3U8File([FromRoute] Guid id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return File(
            cameraService.GetM3U8File(id),
            "application/octet-stream",
            enableRangeProcessing: true
        );
    }

    [HttpGet("{id}/stream/{tsFileName}")]
    public IActionResult GetTsFile([FromRoute] Guid id, [FromRoute] string tsFileName)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return File(
            cameraService.GetTsFile(id, tsFileName),
            "application/octet-stream",
            enableRangeProcessing: true
        );
    }

    [HttpGet("test/ai")]
    public void RunAI()
    {
        aiService.RunAI();
    }

    [HttpGet("test/ai/kill")]
    public void KillAi()
    {
        aiService.KillAI();
    }
}
