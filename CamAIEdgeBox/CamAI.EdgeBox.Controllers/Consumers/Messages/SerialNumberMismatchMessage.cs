using CamAI.EdgeBox.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn("SerialNumberMismatchMessage")]
public class SerialNumberMismatchMessage : RoutingKeyMessage
{
    public string SerialNumber { get; set; } = null!;
}
