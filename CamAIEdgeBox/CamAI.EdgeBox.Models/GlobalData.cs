namespace CamAI.EdgeBox.Models;

public static class GlobalData
{
    public static Shop? Shop { get; set; } = null!;
    public static Brand? Brand { get; set; } = null!;
    public static DbEdgeBox? EdgeBox { get; set; } = null!;
    public static List<Camera> Cameras { get; set; } = [];
    public static string? Version { get; set; }
    public static string? MacAddress { get; set; }
    public static string? OsName { get; set; }
    public static Guid EdgeBoxId { get; set; }
    public static string InitializeRequestId { get; set; }
}
