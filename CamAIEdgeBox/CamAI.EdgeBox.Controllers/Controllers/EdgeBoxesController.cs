using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DbEdgeBoxesController : Controller
{
    [HttpGet]
    public DbEdgeBox? GetEdgeBox()
    {
        return GlobalData.EdgeBox;
    }

    [HttpPut]
    public DbEdgeBox UpsertEdgeBox([FromBody] DbEdgeBox edgeBoxDto)
    {
        var edgeBox = EdgeBoxService.UpsertEdgeBox(edgeBoxDto);
        return edgeBox;
    }
}
