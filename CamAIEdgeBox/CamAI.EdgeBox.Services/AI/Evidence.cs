namespace CamAI.EdgeBox.Services.AI;

public class Evidence
{
    public Uri? Uri { get; set; }
    public EvidenceType EvidenceType { get; set; }
    public Guid CameraId { get; set; }
    public Guid EdgeBoxId { get; set; }
}
