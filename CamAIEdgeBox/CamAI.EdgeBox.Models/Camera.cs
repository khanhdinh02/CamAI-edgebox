namespace CamAI.EdgeBox.Models;

public class Camera : BaseEntity
{
    public string Name { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public int Port { get; set; }
}