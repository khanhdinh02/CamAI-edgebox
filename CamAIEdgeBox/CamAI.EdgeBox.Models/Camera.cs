using System.ComponentModel.DataAnnotations;

namespace CamAI.EdgeBox.Models;

public class Camera : BaseEntity
{
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Zone { get; set; }

    [StringLength(255)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(10)]
    public string Protocol { get; set; } = null!;
    public int Port { get; set; }
    public string Host { get; set; } = null!;
    public string Path { get; set; } = null!;
    public int Index { get; set; }

    public CameraStatus Status { get; set; } = CameraStatus.New;
}

public enum CameraStatus
{
    New = 1,
    Connected = 2,
    Disconnected = 3,
}
