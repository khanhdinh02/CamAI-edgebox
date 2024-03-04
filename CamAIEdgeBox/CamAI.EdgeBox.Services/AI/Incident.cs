namespace CamAI.EdgeBox.Services.AI;

public class Incident
{
    public IncidentType IncidentType { get; set; }
    public DateTime Time { get; set; }
    public virtual ICollection<Evidence> Evidences { get; set; } = new HashSet<Evidence>();
}
