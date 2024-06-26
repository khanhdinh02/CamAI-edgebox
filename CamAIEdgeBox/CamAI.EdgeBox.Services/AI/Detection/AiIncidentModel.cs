namespace CamAI.EdgeBox.Services.AI.Detection;

public abstract class AiIncidentModel : AiModel
{
    public abstract IncidentType Type { get; }
    public List<CalculationEvidence> Evidences { get; } = [];

    public string NewEvidenceName() => $"{Type}_{Id:N}_{Evidences.Count}";
}
