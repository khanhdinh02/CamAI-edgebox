namespace CamAI.EdgeBox.Services.AI;

public class AiConfiguration
{
    public static string Section => "AI";
    public string OutputDirectory { get; set; } = null!;
    public string OutputSeparator { get; set; } = null!;
    public string OutputFile { get; set; } = null!;
    public ClassifierConfiguration Classifier { get; set; } = null!;
    public DetectionConfiguration Detection { get; set; } = null!;
}

public class ClassifierConfiguration
{
    public int Interval { get; set; }
}

public class DetectionConfiguration
{
    public double MinScore { get; set; }
    public int MinDuration { get; set; }
}
