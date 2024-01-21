namespace CamAI.EdgeBox.Models;

public class DbEdgeBox : BaseEntity
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? Model { get; set; }
    public string? Version { get; set; }

    public LookUp EdgeBoxStatus { get; set; } = null!;
    public LookUp EdgeBoxLocation { get; set; } = null!;
}
