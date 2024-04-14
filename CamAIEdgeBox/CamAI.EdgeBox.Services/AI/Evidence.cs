using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.AI;

public class Evidence
{
    public byte[] Content { get; set; } = null!;
    public EvidenceType EvidenceType { get; set; }
    public Camera Camera { get; set; }
    public Guid EdgeBoxId { get; set; } = GlobalData.EdgeBox!.Id;
}
