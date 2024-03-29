using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn("StreamingMessage")]
public class StreamingMessage
{
    public Guid CameraId { get; set; }
    public Uri? HttpRelayUri { get; set; }
}
