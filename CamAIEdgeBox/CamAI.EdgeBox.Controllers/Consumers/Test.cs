using CamAI.EdgeBox.MassTransit;

namespace CamAI.EdgeBox.Consumers;

[QueuePublisher("test")]
public class Test
{
    public string Something { get; set; }
}
