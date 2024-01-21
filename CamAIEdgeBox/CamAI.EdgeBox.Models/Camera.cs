using System.ComponentModel.DataAnnotations;

namespace CamAI.EdgeBox.Models;

public class Camera : BaseEntity
{
    [StringLength(255)]
    public string Name { get; set; } = null!;
    [StringLength(255)]
    public string? Area { get; set; }

    [StringLength(255)]
    public string Username { get; set; } = null!;
    [StringLength(255)]
    public string Password { get; set; } = null!;
    [StringLength(10)]
    public string Protocol { get; set; } = null!;
    public int Port { get; set; }
    public Uri IpAddress { get; set; } = null!;
    public int Index { get; set; }
}