using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using MassTransit;
using RabbitMQ.Client;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

[Consumer(
    queueName: "ActivateConsumer_{MachineName}",
    exchangeName: Constants.ActivateEdgeBox,
    exchangeType: ExchangeType.Direct,
    routingKey: "{EdgeBoxId}"
)]
public class ActivatedEdgeBoxConsumer(
    ILogger<ActivatedEdgeBoxConsumer> logger,
    EdgeBoxService edgeBoxService,
    IPublishEndpoint bus,
    AiService aiService
) : IConsumer<ActivatedEdgeBoxMessage>
{
    public Task Consume(ConsumeContext<ActivatedEdgeBoxMessage> context)
    {
        logger.LogInformation("Message is received: {0}", context.Message.Message);
        var confirmedEdgeBoxActivationMessage = new ConfirmedEdgeBoxActivationMessage
        {
            EdgeBoxId = GlobalData.EdgeBox!.Id,
            IsActivatedSuccessfully = true
        };

        if (edgeBoxService.ActivateEdgeBox() == 0)
            confirmedEdgeBoxActivationMessage.IsActivatedSuccessfully = false;
        bus.Publish(confirmedEdgeBoxActivationMessage);

        if (confirmedEdgeBoxActivationMessage.IsActivatedSuccessfully)
            aiService.RunAi();

        return Task.CompletedTask;
    }
}
