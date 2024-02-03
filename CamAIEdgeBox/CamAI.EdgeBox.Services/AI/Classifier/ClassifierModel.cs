using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services.MassTransit;

namespace CamAI.EdgeBox.Services.AI;

[Publisher(Constants.Classifier)]
public class ClassifierModel
{
    public DateTime Time { get; set; }
    public List<ClassifierResult> Results { get; set; } = null!;
    public int Total { get; set; }
    public Guid ShopId { get; set; }
}

public class ClassifierResult
{
    public ActionType ActionType { get; set; }
    public int Count { get; set; }
}

public enum ActionType
{
    Idle = 0,
    Walking = 1,
    Phone = 2,
    Laptop = 3
}

public static class EnumConversion
{
    public static readonly IReadOnlyDictionary<string, ActionType> ActionTypes = new Dictionary<string, ActionType>()
    {
        { "idle", ActionType.Idle },
        { "walking", ActionType.Walking },
        { "phone", ActionType.Phone },
        { "laptop", ActionType.Laptop }
    };
}