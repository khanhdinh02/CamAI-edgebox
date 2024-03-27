using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.AI;

public abstract class AiModel
{
    public Zone Zone { get; set; }
    public int AiId { get; init; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime StartTime { get; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
}
