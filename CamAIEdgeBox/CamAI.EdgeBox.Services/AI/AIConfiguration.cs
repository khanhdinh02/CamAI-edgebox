﻿namespace CamAI.EdgeBox.Services.AI;

public class AiConfiguration
{
    public static string Section => "AI";
    public string BaseDirectory { get; set; } = null!;
    public string OutputSeparator { get; set; } = null!;
    public string OutputFile { get; set; } = null!;
    public string EvidenceOutputDir { get; set; } = null!;
    public HumanCountConfiguration HumanCount { get; set; } = null!;
    public DetectionConfiguration Detection { get; set; } = null!;
    public UniformConfiguration Uniform { get; set; } = null!;
}

public class HumanCountConfiguration
{
    public int Interval { get; set; }
}

public class DetectionConfiguration
{
    public double MinScore { get; set; }
    public int MinDuration { get; set; }
}

public class UniformConfiguration
{
    public int MinUniformCount { get; set; }
    public int Duration { get; set; }
}
