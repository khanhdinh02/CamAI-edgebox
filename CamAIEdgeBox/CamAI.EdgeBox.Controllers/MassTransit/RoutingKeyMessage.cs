namespace CamAI.EdgeBox.MassTransit;

public abstract class RoutingKeyMessage
{
    public string RoutingKey { get; set; } = null!;
}
