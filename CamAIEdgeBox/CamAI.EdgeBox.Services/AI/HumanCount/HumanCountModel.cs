using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services.MassTransit;

namespace CamAI.EdgeBox.Services.AI;

[Publisher(Constants.HumanCount)]
public class HumanCountModel
{
    public DateTime Time { get; set; }
    public int Count { get; set; }
}
