namespace CamAI.EdgeBox.MassTransit;

[AttributeUsage(AttributeTargets.Class)]
public class MessageQueueEndpointAttribute(string queueName) : Attribute
{
    public string QueueName => queueName;
}
