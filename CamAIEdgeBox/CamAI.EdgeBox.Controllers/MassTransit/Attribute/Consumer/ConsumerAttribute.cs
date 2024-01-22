namespace CamAI.EdgeBox.MassTransit;

public abstract class ConsumerAttribute(string template) : ServiceBusEndpointAttribute(template);
