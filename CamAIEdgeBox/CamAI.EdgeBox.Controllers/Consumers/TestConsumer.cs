using CamAI.EdgeBox.MassTransit;
using MassTransit;
using RabbitMQ.Client;

namespace CamAI.EdgeBox.Consumers;

[Consumer("queueName", "directExchange", "routingKey", ExchangeType.Direct)]
public class TestConsumer : IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
    {
        return Task.CompletedTask;
    }
}
