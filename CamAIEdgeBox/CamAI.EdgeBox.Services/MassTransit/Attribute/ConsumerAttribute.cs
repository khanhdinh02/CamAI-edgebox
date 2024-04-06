using RabbitMQ.Client;

namespace CamAI.EdgeBox.MassTransit;

public class ConsumerAttribute(
    string queueName,
    string? exchangeName = null,
    string? routingKey = null,
    string exchangeType = ExchangeType.Fanout,
    int timeToLive = 0
) : MessageQueueEndpointAttribute(queueName)
{
    public override string QueueName => $"Consumer:{FormatTemplate(Template)}";
    public string RoutingKey => FormatTemplate(routingKey ?? Template);
    public string ExchangeName => $"Publisher:{FormatTemplate(exchangeName ?? Template)}";
    public string ExchangeType => exchangeType;
    public int TimeToLive => timeToLive;
}
