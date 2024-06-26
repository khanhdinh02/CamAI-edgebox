﻿namespace CamAI.EdgeBox.MassTransit;

public class PublisherAttribute(string queueName) : MessageQueueEndpointAttribute(queueName)
{
    public override string QueueName => $"Publisher:{FormatTemplate(Template)}";
    public Uri Uri => new("exchange:" + QueueName);
}
