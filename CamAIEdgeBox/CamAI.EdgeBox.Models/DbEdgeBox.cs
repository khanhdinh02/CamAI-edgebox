namespace CamAI.EdgeBox.Models;

public class DbEdgeBox : BaseEntity
{
    // TODO [Duy]: Do we want to sync edge box data from server. Should it be opposite?
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public EdgeBoxStatus EdgeBoxStatus { get; set; }
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
