using System.Collections.Concurrent;
using CamAI.EdgeBox.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class DetectionProcessor : IDisposable
{
    private bool disposed;
    private readonly BlockingCollection<List<ClassifierOutputModel>> classifierOutputs =
        new(new ConcurrentQueue<List<ClassifierOutputModel>>(), 1000);
    private readonly DetectionConfiguration detection;

    private readonly ISendEndpoint bus;

    private readonly Dictionary<int, DetectionScoreModel> scoreCalculation = [];

    public DetectionProcessor(
        ClassifierWatcher watcher,
        IOptions<AiConfiguration> configuration,
        ISendEndpoint bus
    )
    {
        watcher.Notifier += ReceiveData;
        detection = configuration.Value.Detection;
        this.bus = bus;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        while (!classifierOutputs.IsCompleted)
        {
            var outputs = classifierOutputs.Take(cancellationToken);

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
                        interval.MaxBreakTime = CalculateMaxBreakTime(interval);
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

            // calculate incident
            foreach (
                var (id, calculation) in scoreCalculation.Where(x =>
                    x.Value.Score() >= detection.MinScore
                    && x.Value.TotalTime() >= detection.MinDuration
                )
            )
            {
                var incident = new Incident
                {
                    Time = DateTime.Now,
                    IncidentType = IncidentType.Phone,
                    Evidences =
                    [
                        new Evidence
                        {
                            EdgeBoxId = GlobalData.EdgeBox!.Id,
                            EvidenceType = EvidenceType.Image,
                            // TODO: get evidence uri
                            // TODO: implement API to get evidence
                            Uri = new Uri("localhost")
                        }
                    ]
                };

                // TODO: extract evidence
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                bus.Send(incident, cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
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
        return m < 3 ? 3 : m;
    }

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        // TODO: what if output count is still 0 after 10 seconds
        var phoneOutput = output.Where(x => x.Data.Label == ActionType.Phone).ToList();
        if (phoneOutput.Count > 0)
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
