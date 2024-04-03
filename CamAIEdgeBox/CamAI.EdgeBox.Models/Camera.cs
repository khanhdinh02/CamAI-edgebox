using System.ComponentModel.DataAnnotations;

namespace CamAI.EdgeBox.Models;

public class Camera : BaseEntity
{
    public Guid ShopId { get; set; } = GlobalData.Shop!.Id;

    [StringLength(255)]
    public string Name { get; set; } = null!;

    public Zone Zone { get; set; }

    [StringLength(255)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(10)]
    public string Protocol { get; set; } = null!;
    public int Port { get; set; }
    public string Host { get; set; } = null!;
    public string Path { get; set; } = null!;
    public bool WillRunAI { get; set; }

    public CameraStatus Status { get; set; } = CameraStatus.New;
}

public enum CameraStatus
{
    New = 1,
    Connected = 2,
    Disconnected = 3,
}

public enum Zone
{
    Cashier = 0,
    Customer = 1
}
