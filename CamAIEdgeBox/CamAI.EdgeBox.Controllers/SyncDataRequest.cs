using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Controllers;

[Publisher(Constants.SyncDataRequest)]
[MessageUrn(nameof(SyncDataRequest))]
public class SyncDataRequest
{
    public Guid EdgeBoxId { get; set; }
}
