
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace CamAI.EdgeBox.Controllers.BackgroundServices;

public class CpuTrackingService(IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = provider.CreateScope();
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500, stoppingToken);
            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var memories = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            memories.Set("CpuUsage", cpuUsageTotal);
        }
    }
}
