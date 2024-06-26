using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Services.AI;

[Publisher("Detection")]
[MessageUrn("ReceivedIncident")]
public class Incident
{
    public Guid EdgeBoxId { get; } = GlobalData.EdgeBox!.Id;
    public Guid Id { get; set; }
    public int AiId { get; set; }
    public IncidentType IncidentType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public virtual ICollection<Evidence> Evidences { get; set; } = new HashSet<Evidence>();
}
