namespace CamAI.EdgeBox.Services.AI.Detection;

public abstract class AiIncidentModel : AiModel
{
    public List<CalculationEvidence> Evidences { get; } = [];

    public string NewEvidenceName() => Id.ToString("N") + Evidences.Count;
}
