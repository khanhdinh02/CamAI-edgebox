using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
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
        if (GlobalData.InitializeRequestId != context.Message.RequestId)
        {
            Console.WriteLine(
                "Receive serial mismatch but different request id, received {0}, expected request {1}",
                context.Message.RequestId,
                GlobalData.InitializeRequestId
            );
            return Task.CompletedTask;
        }

        Console.WriteLine(
            "Edge box serial number mismatch, current edge box serial number is {0}, but serial number in server is {1}",
            IOUtil.GetSerialNumber(),
            context.Message.SerialNumber
        );
        Environment.Exit(60);
        return Task.CompletedTask;
    }
}
