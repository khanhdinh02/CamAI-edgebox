using System.Collections.Concurrent;
using CamAI.EdgeBox.Services.AI.Detection;
using MassTransit;
using Serilog;

namespace CamAI.EdgeBox.Services.AI;

public class InteractionProcessor : IDisposable
{
    private bool disposed;

    // TODO: with employee or without employee
    private readonly BlockingCollection<List<ClassifierOutputModel>> classifierOutputs =
        new(new ConcurrentQueue<List<ClassifierOutputModel>>(), 1000);
    private readonly InteractionConfiguration interaction;
    private readonly IPublishEndpoint bus;
    private readonly Dictionary<int, InteractionModel> calculations = [];

    public InteractionProcessor(
        ClassifierWatcher watcher,
        InteractionConfiguration interaction,
        IPublishEndpoint bus
    )
    {
        Log.Information("Create interaction processor");
        watcher.Notifier += ReceiveData;
        this.interaction = interaction;
        this.bus = bus;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        Log.Information("Running interaction processor");
        while (!classifierOutputs.IsCompleted)
        {
            var outputs = classifierOutputs.Take(cancellationToken);
            var personToRemove = new List<InteractionModel>();
            // update calculation
            foreach (var (id, calculation) in calculations)
            {
                if (!outputs.Exists(x => x.Id == id))
                {
                    calculation.BreakCount += 1;
                    if (calculation.BreakCount > interaction.MaxBreak)
                    {
                        calculation.EndTime = DateTime.UtcNow;
                        personToRemove.Add(calculation);
                    }
                }
                else
                {
                    calculation.Count += calculation.BreakCount;
                    calculation.BreakCount = 0;
                }
            }

            // add new model to dictionary
            var scoreIds = calculations.Keys.ToList();
            var newOutputs = outputs.Where(x => !scoreIds.Contains(x.Id));
            foreach (var output in newOutputs)
            {
                // TODO: add person type, zone
                var model = new InteractionModel { AiId = output.Id, };
                calculations.TryAdd(model.AiId, model);
            }

            // send message
            foreach (var (id, calculation) in calculations)
                if (calculation.Count > interaction.MinDuration && calculation.EndTime != null)
                    await bus.SendIncident(calculation, cancellationToken);

            // remove calculation
            foreach (var d in personToRemove)
                calculations.Remove(d.AiId);
        }
    }

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
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