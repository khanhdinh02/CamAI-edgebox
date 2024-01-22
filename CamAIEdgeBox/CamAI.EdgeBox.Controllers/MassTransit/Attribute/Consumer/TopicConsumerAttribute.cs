namespace CamAI.EdgeBox.MassTransit;

/// <summary>
/// This attribute indicates what endpoint the consumer will consume
/// </summary>
public class TopicConsumerAttribute(string template) : ConsumerAttribute(template);
