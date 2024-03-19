namespace CamAI.EdgeBox.Services.AI.Detection;

public abstract class AiIncidentModel
{
    public int AiId { get; init; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<CalculationEvidence> Evidences { get; } = [];
    public DateTime StartTime { get; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }

    public string NewEvidenceName() => Id.ToString("N") + Evidences.Count;
}
