using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services;
using MassTransit;
using RabbitMQ.Client;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

[Consumer(queueName: $"ActivateConsumer", exchangeName: Constants.ActivateEdgeBox, exchangeType: ExchangeType.Direct, routingKey: "{EdgeBoxId}")]
public class ActivatedEdgeBoxConsumer(ILogger<ActivatedEdgeBoxConsumer> logger, AIService aiService, EdgeBoxService edgeBoxService, IPublishEndpoint bus) : IConsumer<ActivatedEdgeBoxMessage>
{
    public Task Consume(ConsumeContext<ActivatedEdgeBoxMessage> context)
    {
        logger.LogInformation("Message is received: {0}", context.Message.Message);
        var confirmedEdgeBoxActivationMessage = new ConfirmedEdgeBoxActivationMessage
        {
            EdgeBoxId = GlobalData.EdgeBox!.Id,
            IsActivatedSuccessfully = true
        };
        try
        {
            aiService.RunAI();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }

        if (edgeBoxService.ActivateEdgeBox() == 0)
            confirmedEdgeBoxActivationMessage.IsActivatedSuccessfully = false;
        return bus.Publish(confirmedEdgeBoxActivationMessage);
    }
}
