using CamAI.EdgeBox.MassTransit;
using MassTransit;
using RabbitMQ.Client;

namespace CamAI.EdgeBox.Consumers;

[Consumer("otherQueue", "directExchange", "routingKey2", ExchangeType.Direct)]
public class TestConsumer2 : IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
    {
        return Task.CompletedTask;
    }
}
