using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CamerasController(CameraService cameraService) : Controller
{
    [HttpGet]
    public List<Camera> GetCameras() => GlobalData.Cameras;

    [HttpGet("{id}")]
    public Camera GetCamera([FromRoute] Guid id) =>
        GlobalData.Cameras.Find(x => x.Id == id) ?? throw new Exception("Camera not found");

    [HttpPost]
    public Camera AddCamera([FromBody] Camera cameraDto)
    {
        cameraDto.Id = Guid.NewGuid();
        return cameraService.UpsertCamera(cameraDto);
    }

    [HttpPut("{id}")]
    public Camera UpdateCamera([FromRoute] Guid id, [FromBody] Camera cameraDto)
    {
        // Check if camera exist
        cameraService.GetCamera(id);

        cameraDto.Id = id;
        return cameraService.UpsertCamera(cameraDto);
    }

    [HttpDelete("{id}")]
    public void DeleteCamera([FromRoute] Guid id)
    {
        cameraService.DeleteCamera(id);
    }

    [HttpPut("{id}/connection")]
    public void CheckConnection([FromRoute] Guid id)
    {
        cameraService.CheckCameraConnection(id);
    }
}
