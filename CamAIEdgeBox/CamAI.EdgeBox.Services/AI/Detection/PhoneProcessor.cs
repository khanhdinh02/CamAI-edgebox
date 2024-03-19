using System.Collections.Concurrent;
using CamAI.EdgeBox.Services.AI.Detection;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;

namespace CamAI.EdgeBox.Services.AI;

public class PhoneProcessor : IDisposable
{
    private bool disposed;
    private readonly BlockingCollection<List<ClassifierOutputModel>> classifierOutputs =
        new(new ConcurrentQueue<List<ClassifierOutputModel>>(), 1000);
    private readonly PhoneConfiguration phone;
    private readonly RtspExtension rtsp;
    private readonly IPublishEndpoint bus;

    private readonly Dictionary<int, PhoneModel> calculations = [];

    public PhoneProcessor(
        ClassifierWatcher watcher,
        RtspExtension rtsp,
        PhoneConfiguration phone,
        IPublishEndpoint bus
    )
    {
        watcher.Notifier += ReceiveData;
        this.phone = phone;
        this.rtsp = rtsp;
        this.bus = bus;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        while (!classifierOutputs.IsCompleted)
        {
            var outputs = classifierOutputs.Take(cancellationToken);

            var phoneToRemove = new List<PhoneModel>();
            // update calculation
            foreach (var (id, calculation) in calculations)
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
                        interval.MaxBreakTime = CalculateMaxBreakTime(interval);
                        interval.BreakTime = interval.SkipCount;
                    }
                    else
                    {
                        interval.BreakTime = +1;
                        if (interval.BreakTime >= interval.MaxBreakTime)
                        {
                            calculation.EndTime = DateTime.UtcNow;
                            phoneToRemove.Add(calculation);
                        }
                    }
                }
            }

            // add new calculation
            var scoreIds = calculations.Keys.ToList();
            var newOutputs = outputs.Where(x => !scoreIds.Contains(x.Id));
            foreach (var output in newOutputs)
            {
                var model = new PhoneModel
                {
                    AiId = output.Id,
                    Intervals = [new DetectionInterval(output.Data.Score)]
                };
                calculations.TryAdd(model.AiId, model);
            }

            // calculate incident
            foreach (var (id, calculation) in calculations)
            {
                var interval = calculation.Intervals[^1];
                if (
                    (interval.Scores.Count == 4 || interval.Scores.Count % 20 == 0)
                    && interval.MaxBreakTime == 0
                )
                    rtsp.CaptureEvidence(calculation);

                if (
                    calculation.Score >= phone.MinScore
                    && calculation.TotalTime >= phone.MinDuration
                    && calculation.ShouldBeSend()
                )
                    await bus.SendIncident(calculation, cancellationToken);
            }

            // remove calculation
            foreach (var d in phoneToRemove)
                calculations.Remove(d.AiId);
        }
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

    private static int CalculateMaxBreakTime(DetectionInterval interval)
    {
        var t = interval.Scores.Count;
        var score = interval.IntervalScore();
        var m = Convert.ToInt32(0.5 * t / (2 * Math.Log(7 - 6 * score) + 1));
        return m switch
        {
            < 3 => 3,
            > 90 => 90,
            _ => m
        };
    }

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        var phoneOutput = output.Where(x => x.Data.Label == ActionType.Phone).ToList();
        classifierOutputs.Add(phoneOutput);
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
