using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(EdgeBoxUpdateMessage))]
public class EdgeBoxUpdateMessage : BaseUpdateMessage
{
    public string? SerialNumber { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int MaxNumberOfRunningAi { get; set; }
    public EdgeBoxActivationStatus ActivationStatus { get; set; }
}

public enum EdgeBoxActivationStatus
{
    NotActivated = 0,
    Activated = 1,
    Pending = 2,
    Failed = 3
}
