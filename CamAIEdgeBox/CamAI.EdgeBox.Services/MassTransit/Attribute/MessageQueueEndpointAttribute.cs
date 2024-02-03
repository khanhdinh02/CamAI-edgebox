namespace CamAI.EdgeBox.MassTransit;

[AttributeUsage(AttributeTargets.Class)]
public abstract class MessageQueueEndpointAttribute(string queueName) : Attribute
{
    protected readonly string QName = queueName;
    public abstract string QueueName { get; }
}
