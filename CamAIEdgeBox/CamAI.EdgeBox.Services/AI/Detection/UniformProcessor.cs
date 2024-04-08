using System.Collections.Concurrent;
using CamAI.EdgeBox.Services.AI.Detection;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Serilog;

namespace CamAI.EdgeBox.Services.AI.Uniform;

public class UniformProcessor : IDisposable
{
    private bool disposed;
    private readonly BlockingCollection<List<ClassifierOutputModel>> classifierOutputs =
        new(new ConcurrentQueue<List<ClassifierOutputModel>>(), 1000);
    private readonly UniformConfiguration uniform;
    private readonly AiProcessWrapper.AiProcessUtil aiProcessUtil;
    private readonly IPublishEndpoint bus;
    private readonly Dictionary<int, UniformModel> uniformCalculation = [];

    public UniformProcessor(
        ClassifierWatcher watcher,
        AiProcessWrapper.AiProcessUtil aiProcessUtil,
        UniformConfiguration uniform,
        IPublishEndpoint bus
    )
    {
        Log.Information("Create uniform processor");
        this.uniform = uniform;
        this.bus = bus;
        this.aiProcessUtil = aiProcessUtil;
        watcher.Notifier += ReceiveData;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        Log.Information("Running uniform processor");
        while (!classifierOutputs.IsCompleted)
        {
            var outputs = classifierOutputs.Take(cancellationToken);

            var uniformToRemove = new List<UniformModel>();
            // update calculation
            foreach (var (id, calculation) in uniformCalculation)
            {
                var output = outputs.Find(x => x.Id == id);
                if (output != null)
                    calculation.PositiveCount += 1;
                else
                {
                    calculation.NegativeCount += 1;
                    if (calculation.PositiveRatio < uniform.Ratio)
                    {
                        calculation.EndTime = DateTime.Now;
                        uniformToRemove.Add(calculation);
                    }
                }
            }

            // add new calculation
            var scoreIds = uniformCalculation.Keys.ToList();
            var newOutputs = outputs.Where(x => !scoreIds.Contains(x.Id));
            foreach (var output in newOutputs)
            {
                var model = new UniformModel { AiId = output.Id, };
                uniformCalculation.TryAdd(model.AiId, model);
            }

            // calculate incident
            foreach (var (id, calculation) in uniformCalculation)
            {
                if (calculation.PositiveRatio < uniform.Ratio)
                    continue;

                // capture evidence
                if (
                    calculation.Evidences.Count < 6
                    && (calculation.TotalCount == 4 || calculation.TotalCount % 30 == 0)
                )
                    aiProcessUtil.CaptureEvidence(calculation);

                // send incident
                if (calculation.TotalCount > uniform.MinDuration && calculation.ShouldBeSend())
                    await bus.SendIncident(calculation, cancellationToken);
            }

            // remove calculation
            foreach (var d in uniformToRemove)
            {
                d.CleanUpEvidence();
                uniformCalculation.Remove(d.AiId);
            }
        }
    }

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        classifierOutputs.Add(
            output.Where(x => x.Data is { Uniform: false, Zone: AiZone.Worker }).ToList()
        );
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
