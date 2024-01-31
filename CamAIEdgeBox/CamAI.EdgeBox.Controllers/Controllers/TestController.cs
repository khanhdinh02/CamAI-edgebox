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
}
