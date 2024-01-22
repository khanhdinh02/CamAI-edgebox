using CamAI.EdgeBox.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(ISendEndpointProvider bus) : ControllerBase
{
    [HttpGet("{name}")]
    public ActionResult<string> TestConnection([FromRoute] string name)
    {
        return $"Hello {name} from edge box";
    }

    [HttpGet("test")]
    public void Test()
    {
        bus.Send(new Test { Something = "ya" });
    }
}
