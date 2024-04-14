using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.AI;

public class CalculationEvidence
{
    public string Path { get; set; } = null!;
    public Camera Camera { get; set; }
    public bool IsSent { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
}
