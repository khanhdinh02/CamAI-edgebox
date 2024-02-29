using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(IPublishEndpoint bus, ILogger<TestController> logger, IMemoryCache cache) : ControllerBase
{
    [HttpGet("{name}")]
    public async Task<ActionResult<string>> TestConnection([FromRoute] string name)
    {
        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //{
        //    //var dir = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().LastIndexOf("/"));
        //    //if (Directory.Exists(@$"{dir}/scripts"))
        //    //    await @$"{dir}/scripts/CpuUsageScript.sh".Bash(logger);
        //    await $"top -bn1".LinuxCmd(logger);
        //}
        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //{
        //    await "wmic cpu get loadpercentage".WindowsPrompt(logger);
        //}
        logger.LogInformation($"CPU usage: {cache.Get("CpuUsage")}");
        return $"Hello {name} from edge box";
    }
}
