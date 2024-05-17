using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using RabbitMQ.Client;
using Serilog;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

[Consumer(
    queueName: "SerialMismatch_{MachineName}",
    exchangeName: Constants.SerialNumberMismatch,
    exchangeType: ExchangeType.Direct,
    routingKey: "{EdgeBoxId}",
    timeToLive: 10
)]
public class SerialNumberMismatchConsumer : IConsumer<SerialNumberMismatchMessage>
{
    public Task Consume(ConsumeContext<SerialNumberMismatchMessage> context)
    {
        Log.Fatal(
            "Edge box serial number mismatch, current edge box serial number is {SerialNumber}, but serial number in server is {ServerSerialNumber}",
            IOUtil.GetSerialNumber(),
            context.Message.SerialNumber
        );
        Environment.Exit(60);
        return Task.CompletedTask;
    }
}
