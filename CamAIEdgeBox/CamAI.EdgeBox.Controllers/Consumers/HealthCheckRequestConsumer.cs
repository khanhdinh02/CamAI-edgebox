using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Consumers;

[Consumer("{EdgeBoxId}_HealthCheck", Constants.HealthCheck, timeToLive: 5)]
public class HealthCheckRequestConsumer(AiService aiService, IPublishEndpoint bus)
    : IConsumer<HealthCheckRequestMessage>
{
    public Task Consume(ConsumeContext<HealthCheckRequestMessage> context)
    {
        var (expectedNumOfAi, numOfRunningAi) = aiService.GetRunningAIStatus();
        if (
            GlobalData.EdgeBox!.EdgeBoxStatus == EdgeBoxStatus.Active
            && expectedNumOfAi > numOfRunningAi
        )
        {
            bus.Publish(
                new HealthCheckResponseMessage
                {
                    EdgeBoxId = GlobalData.EdgeBox.Id,
                    Status = EdgeBoxInstallStatus.Unhealthy,
                    Reason = $"Only {numOfRunningAi} out of {expectedNumOfAi} is running"
                }
            );
        }
        else
            bus.Publish(
                new HealthCheckResponseMessage
                {
                    EdgeBoxId = GlobalData.EdgeBox.Id,
                    Status = EdgeBoxInstallStatus.Working
                }
            );
        return Task.CompletedTask;
    }
}