using System.Runtime.InteropServices;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(IPublishEndpoint bus, ILogger<TestController> logger) : ControllerBase
{
    [HttpGet("{name}")]
    public async Task<ActionResult<string>> TestConnection([FromRoute] string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            await $"scripts/CpuUsageScript.sh".Bash(logger);
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            await "wmic cpu get loadpercentage".WindowsPrompt(logger);
        }
        return $"Hello {name} from edge box";
    }
}
