using CamAI.EdgeBox.Services.AI.Detection;

namespace CamAI.EdgeBox.Services.AI;

public class InteractionModel : AiIncidentModel
{
    public PersonType PersonType { get; set; }
    public int Count { get; set; }
    public int BreakCount { get; set; }
}

public enum PersonType
{
    Employee = 0,
    Customer = 1
}
