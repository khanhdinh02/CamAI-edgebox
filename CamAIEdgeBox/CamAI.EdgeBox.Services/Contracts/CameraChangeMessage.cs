using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Services.Contracts;

[Publisher(Constants.CameraChange)]
[MessageUrn("CameraChangeMessage")]
public class CameraChangeMessage
{
    public Camera Camera { get; set; } = null!;
    public Action Action { get; set; }
}

public enum Action
{
    Upsert = 1,
    Delete = 2,
}
