using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(ActivatedEdgeBoxMessage))]
public class ActivatedEdgeBoxMessage
{
    public string Message { get; set; } = String.Empty;
}