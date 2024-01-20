using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CamerasController(CameraService cameraService) : Controller
{
    [HttpGet]
    public List<Camera> GetCameras()
    {
        return cameraService.GetCamera();
    }
}