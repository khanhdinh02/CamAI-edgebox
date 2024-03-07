using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.AI;

public class Evidence
{
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public EvidenceType EvidenceType { get; set; }
    public Guid CameraId { get; set; } = GlobalData.Cameras[0].Id;
    public Guid EdgeBoxId { get; set; } = GlobalData.EdgeBox!.Id;
}
