namespace CamAI.EdgeBox.Models;

public class Camera : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Area { get; set; }
    
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Protocol { get; set; } = null!;
    public int Port { get; set; }
    public Uri IpAddress { get; set; } = null!;
    public int Index { get; set; }
}