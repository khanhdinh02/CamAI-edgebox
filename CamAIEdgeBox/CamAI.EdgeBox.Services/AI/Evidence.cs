using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.AI;

public class Evidence
{
    public byte[] Content { get; set; } = null!;
    public EvidenceType EvidenceType { get; set; }
    public Guid CameraId { get; set; }
    public Guid EdgeBoxId { get; set; } = GlobalData.EdgeBox!.Id;
}
