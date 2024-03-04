namespace CamAI.EdgeBox.Services.AI;

public class DetectionScoreModel
{
    public int Id { get; set; }
    public List<DetectionInterval> Intervals { get; set; } = [new DetectionInterval()];

    public double Score() => Intervals.Select(x => x.IntervalScore()).Average();

    public int TotalTime() =>
        Intervals
            .Select(x =>
            {
                var skipTime = x.BreakTime != 0 ? x.BreakTime : x.SkipCount;
                return skipTime + x.Scores.Count;
            })
            .Sum();
}

public class DetectionInterval
{
    public DetectionInterval() { }

    public DetectionInterval(double score)
    {
        Scores = [score, 1 - score];
    }

    public List<double> Scores { get; set; } = new(1);
    public int MaxBreakTime { get; set; }
    public int BreakTime { get; set; }
    public int SkipCount { get; set; }

    public double IntervalScore()
    {
        var takeAmount = Scores.Count * 2 / 3;
        return Scores.Take(takeAmount).Sum();
    }
}
