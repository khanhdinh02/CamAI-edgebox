using CamAI.EdgeBox.Services.AI.Detection;

namespace CamAI.EdgeBox.Services.AI.Uniform;

public class UniformModel : AiIncidentModel
{
    public override IncidentType Type => IncidentType.Uniform;
    public int PositiveCount { get; set; }
    public int NegativeCount { get; set; }

    public double PositiveRatio => (double)PositiveCount / (NegativeCount + PositiveCount);
    public int TotalCount => PositiveCount + NegativeCount;
}
