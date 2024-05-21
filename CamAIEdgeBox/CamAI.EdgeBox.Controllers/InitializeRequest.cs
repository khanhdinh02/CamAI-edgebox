using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Controllers;

[Publisher(Constants.InitializeRequest)]
[MessageUrn(nameof(InitializeRequest))]
public class InitializeRequest
{
    public Guid EdgeBoxId { get; set; }
    public string IpAddress { get; set; } = null!;
    public string MacAddress { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string OperatingSystem { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string RequestId { get; set; } = null!;
}
