using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Services.Contracts;

[Publisher(Constants.CameraChange)]
[MessageUrn("CameraChangeMessage")]
public class CameraChangeMessage
{
    public CameraDto Camera { get; set; } = null!;
    public Action Action { get; set; }

    public static CameraChangeMessage ToUpsertMessage(Camera camera) =>
        new() { Camera = CameraDto.FromCamera(camera), Action = Action.Upsert };

    public static CameraChangeMessage ToDeleteMessage(Guid id) =>
        new()
        {
            Camera = new CameraDto { Id = id },
            Action = Action.Delete
        };
}

public class CameraDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid ShopId { get; set; }
    public Zone Zone { get; set; }

    public static CameraDto FromCamera(Camera camera)
    {
        return new CameraDto
        {
            Id = camera.Id,
            Name = camera.Name,
            Zone = camera.Zone,
            ShopId = GlobalData.Shop!.Id
        };
    }
}

public enum Action
{
    Upsert = 1,
    Delete = 2,
}
