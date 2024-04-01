using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(CameraUpdateConsumer))]
public class CameraUpdateConsumer
{
    public List<Camera> Cameras { get; set; } = null!;
}
