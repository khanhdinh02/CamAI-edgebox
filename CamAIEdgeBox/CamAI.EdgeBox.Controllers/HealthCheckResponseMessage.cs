using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Controllers;

[Publisher(PublisherConstants.HealthCheckResponse)]
[MessageUrn("HealthCheckResponseMessage")]
public class HealthCheckResponseMessage
{
    public Guid EdgeBoxId { get; set; } = GlobalData.EdgeBox!.Id;
    public EdgeBoxInstallStatus Status { get; set; }
    public string? Reason { get; set; }
}

public enum EdgeBoxInstallStatus
{
    Working = 1,
    Unhealthy = 2
}
