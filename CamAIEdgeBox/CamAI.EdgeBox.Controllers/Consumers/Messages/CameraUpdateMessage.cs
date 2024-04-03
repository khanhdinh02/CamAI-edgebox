using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(CameraUpdateMessage))]
public class CameraUpdateMessage
{
    public List<Camera> Cameras { get; set; } = null!;
}
