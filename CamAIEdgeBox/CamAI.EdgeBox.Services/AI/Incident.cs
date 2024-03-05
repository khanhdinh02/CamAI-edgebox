namespace CamAI.EdgeBox.Services.AI;

public class Incident
{
    public Guid Id { get; set; }
    public IncidentType IncidentType { get; set; }
    public DateTime Time { get; set; }
    public virtual ICollection<Evidence> Evidences { get; set; } = new HashSet<Evidence>();
}
