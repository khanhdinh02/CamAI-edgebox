using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Services.AI;

[Publisher("Incident")]
[MessageUrn("ReceivedIncident")]
public class Incident
{
    public Guid EdgeBoxId { get; } = GlobalData.EdgeBox!.Id;
    public Guid Id { get; set; }
    public IncidentType IncidentType { get; set; }
    public DateTime Time { get; set; }
    public virtual ICollection<Evidence> Evidences { get; set; } = new HashSet<Evidence>();
}
