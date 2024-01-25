namespace CamAI.EdgeBox.MassTransit;

public class PublisherAttribute(string queueName) : MessageQueueEndpointAttribute(queueName)
{
    public Uri Uri => new("exchange:" + QueueName);
}
