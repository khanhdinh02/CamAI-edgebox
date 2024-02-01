namespace CamAI.EdgeBox.Models;

public static class GlobalData
{
    public static Shop? Shop { get; set; } = null!;
    public static Brand? Brand { get; set; } = null!;
    public static DbEdgeBox? EdgeBox { get; set; } = null!;
    public static List<Employee> Employees { get; set; } = null!;
    public static List<Camera> Cameras { get; set; } = null!;
}
