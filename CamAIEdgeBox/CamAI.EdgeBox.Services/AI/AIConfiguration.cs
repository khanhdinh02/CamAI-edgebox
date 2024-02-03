namespace CamAI.EdgeBox.Services.AI;

public class AiConfiguration
{
    public static string Section => "AI";
    public string OutputDirectory { get; set; } = null!;
    public string OutputSeparator { get; set; } = null!;
    public ClassifierConfiguration Classifier { get; set; } = null!;
}

public class ClassifierConfiguration
{
    public int Interval { get; set; }
    public string Output { get; set; } = null!;
}
