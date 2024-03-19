using System.Collections.Concurrent;
using CamAI.EdgeBox.Services.AI.Detection;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;

namespace CamAI.EdgeBox.Services.AI.Uniform;

public class UniformProcessor : IDisposable
{
    private bool disposed;
    private readonly BlockingCollection<List<ClassifierOutputModel>> classifierOutputs =
        new(new ConcurrentQueue<List<ClassifierOutputModel>>(), 1000);
    private readonly UniformConfiguration uniform;
    private readonly RtspExtension rtsp;
    private readonly IPublishEndpoint bus;
    private readonly Dictionary<int, UniformModel> uniformCalculation = [];

    public UniformProcessor(
        ClassifierWatcher watcher,
        RtspExtension rtsp,
        UniformConfiguration uniform,
        IPublishEndpoint bus
    )
    {
        this.uniform = uniform;
        this.bus = bus;
        this.rtsp = rtsp;
        watcher.Notifier += ReceiveData;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
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
                        calculation.EndTime = DateTime.UtcNow;
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
                    rtsp.CaptureEvidence(calculation);

                // send incident
                if (calculation.TotalCount > uniform.MinDuration && calculation.ShouldBeSend())
                    await bus.SendIncident(calculation, cancellationToken);
            }

            // remove calculation
            foreach (var d in uniformToRemove)
                uniformCalculation.Remove(d.AiId);
        }
    }

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        // TODO: filter data only in cashier track box and add data
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
