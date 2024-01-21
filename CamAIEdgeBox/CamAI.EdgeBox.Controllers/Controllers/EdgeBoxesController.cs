using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DbEdgeBoxesController(EdgeBoxService edgeBoxService) : Controller
{
    [HttpGet]
    public DbEdgeBox GetEdgeBox()
    {
        return edgeBoxService.GetEdgeBox();
    }

    [HttpPut]
    public DbEdgeBox UpsertEdgeBox([FromBody] DbEdgeBox edgeBox)
    {
        return edgeBoxService.UpsertEdgeBox(edgeBox);
    }
}
