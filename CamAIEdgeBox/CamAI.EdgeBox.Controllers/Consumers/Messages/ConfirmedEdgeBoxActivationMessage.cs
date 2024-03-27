using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[Publisher(PublisherConstants.ConfirmedActivation)]
[MessageUrn(nameof(ConfirmedEdgeBoxActivationMessage))]
public class ConfirmedEdgeBoxActivationMessage
{
    public Guid EdgeBoxId { get; set; }
    public bool IsActivatedSuccessfully { get; set; }
}