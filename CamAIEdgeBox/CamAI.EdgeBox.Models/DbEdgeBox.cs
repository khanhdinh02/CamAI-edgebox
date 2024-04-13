namespace CamAI.EdgeBox.Models;

public class DbEdgeBox : BaseEntity
{
    public string? Name { get; set; }
    public EdgeBoxStatus EdgeBoxStatus { get; set; } = EdgeBoxStatus.Inactive;
    public string? Model { get; set; }
    public string? Version => GlobalData.Version;
    public string SerialNumber { get; set; } = null!;
    public string? MacAddress => GlobalData.MacAddress;
    public string? OsName => GlobalData.OsName;
}

public enum EdgeBoxStatus
{
    Active = 1,
    Inactive = 2,
    Broken = 3,
    Disposed = 4
}
