using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Serilog;

namespace CamAI.EdgeBox.Controllers.BackgroundServices;

public class HealthCheckBackgroundService(IServiceProvider provider, IConfiguration configuration)
    : BackgroundService
{
    private readonly int healthCheckDelay = configuration.GetSection("HealthCheckDelay").Get<int>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // init service
                var scope = provider.CreateScope();

                var aiService = scope.ServiceProvider.GetRequiredService<AiService>();
                var bus = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                var (expectedNumOfAi, numOfRunningAi) = aiService.GetRunningAIStatus();
                if (
                    GlobalData.EdgeBox!.EdgeBoxStatus == EdgeBoxStatus.Active
                    && AiService.IsShopOpen()
                    && expectedNumOfAi > numOfRunningAi
                )
                {
                    await bus.Publish(
                        new HealthCheckResponseMessage
                        {
                            EdgeBoxId = GlobalData.EdgeBox.Id,
                            Status = EdgeBoxInstallStatus.Unhealthy,
                            Reason = $"Only {numOfRunningAi} out of {expectedNumOfAi} is running",
                            IpAddress = NetworkUtil.GetLocalIpAddress()
                        },
                        stoppingToken
                    );
                }
                else
                    await bus.Publish(
                        new HealthCheckResponseMessage
                        {
                            EdgeBoxId = GlobalData.EdgeBox.Id,
                            Status = EdgeBoxInstallStatus.Working,
                            IpAddress = NetworkUtil.GetLocalIpAddress()
                        },
                        stoppingToken
                    );

                await Task.Delay(TimeSpan.FromSeconds(healthCheckDelay), stoppingToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }
    }
}
