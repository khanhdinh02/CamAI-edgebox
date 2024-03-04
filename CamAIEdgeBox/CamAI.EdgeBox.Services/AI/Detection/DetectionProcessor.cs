using System.Collections.Concurrent;

namespace CamAI.EdgeBox.Services.AI;

public class DetectionProcessor : IDisposable
{
    private bool disposed;
    private readonly BlockingCollection<List<ClassifierOutputModel>> classifierOutputs =
        new(new ConcurrentQueue<List<ClassifierOutputModel>>(), 1000);

    private readonly Dictionary<int, DetectionScoreModel> scoreCalculation = [];

    public DetectionProcessor(ClassifierWatcher watcher)
    {
        watcher.Notifier += ReceiveData;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        while (!classifierOutputs.IsCompleted)
        {
            var outputs = classifierOutputs
                .Take(cancellationToken)
                .Where(x => x.Data.Label == ActionType.Phone)
                .ToList();

            var detectionToRemove = new List<DetectionScoreModel>();
            // update calculation
            foreach (var (id, calculation) in scoreCalculation)
            {
                var output = outputs.Find(x => x.Id == id);
                var interval = calculation.Intervals[^1];
                if (output != null)
                {
                    // update interval
                    if (interval.MaxBreakTime == 0)
                    {
                        // if the AI output is not from a break, continue update the scores
                        interval.Scores = CalculateNewScores(output.Data.Score, interval);
                        interval.SkipCount = 0;
                    }
                    else
                        // if the AI output is from a break, add a new interval
                        calculation.Intervals.Add(new DetectionInterval(output.Data.Score));
                }
                else
                {
                    // skip and break time
                    interval.SkipCount += 1;
                    if (interval is { SkipCount: >= 3, MaxBreakTime: 0 })
                    {
                        interval.MaxBreakTime = CalculateMaxBreakTime(
                            interval.Scores.Count,
                            interval.IntervalScore
                        );
                        interval.BreakTime = interval.SkipCount;
                    }
                    else
                    {
                        interval.BreakTime = +1;
                        if (interval.BreakTime >= interval.MaxBreakTime)
                            detectionToRemove.Add(calculation);
                    }
                }
            }

            // remove calculation
            foreach (var d in detectionToRemove)
                scoreCalculation.Remove(d.Id);

            // add new calculation
            var scoreIds = scoreCalculation.Keys.ToList();
            var newOutputs = outputs.Where(x => !scoreIds.Contains(x.Id));
            foreach (var output in newOutputs)
            {
                var model = new DetectionScoreModel
                {
                    Id = output.Id,
                    Intervals = [new DetectionInterval(output.Data.Score)]
                };
                scoreCalculation.TryAdd(output.Id, model);
            }
        }
        // TODO: determine if there is incident
        // TODO: extract evidence
    }

    private static List<double> CalculateNewScores(
        double outputScore,
        DetectionInterval latestInterval
    )
    {
        var newScores = new List<double>(latestInterval.Scores.Count + 1);
        for (var i = 0; i <= latestInterval.Scores.Count; i++)
        {
            var score = 0.0d;
            if (i != 0)
                score += latestInterval.Scores[i - 1] * (1 - outputScore);
            if (i != latestInterval.Scores.Count)
                score += latestInterval.Scores[i] * outputScore;
            newScores.Add(score);
        }

        return newScores;
    }

    private static int CalculateMaxBreakTime(int t, double score)
    {
        var m = Convert.ToInt32(0.5 * t / (2 * Math.Log(7 - 6 * score) + 1));
        return m < 3 ? 3 : m;
    }

    // TODO  send evidence

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        // TODO: what if output count is still 0 after 10 seconds
        if (output.Count > 0)
            classifierOutputs.Add(output);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool _)
    {
        if (disposed)
            return;

        disposed = true;
    }
}

public class DetectionScoreModel
{
    public int Id { get; set; }
    public List<DetectionInterval> Intervals { get; set; } = [new DetectionInterval()];
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

    public double IntervalScore
    {
        get
        {
            var takeAmount = Scores.Count * 2 / 3;
            return Scores.Take(takeAmount).Sum();
        }
    }
}
