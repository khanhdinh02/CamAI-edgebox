namespace CamAI.EdgeBox.Models;

public class DbEdgeBox : BaseEntity
{
    public string Name { get; set; } = null!;
    public EdgeBoxStatus EdgeBoxStatus { get; set; } = EdgeBoxStatus.Inactive;
    public string? Model { get; set; }
    public string? Version { get; set; }
}

public enum EdgeBoxStatus
{
    Active = 1,
    Inactive = 2,
    Broken = 3,
    Disposed = 4
}
