namespace CamAI.EdgeBox.MassTransit;

/// <summary>
/// This attribute indicates what endpoint the consumer will consume
/// </summary>
public class QueueConsumerAttribute(string template) : ConsumerAttribute(template);
