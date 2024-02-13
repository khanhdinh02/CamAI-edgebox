using System.Collections.Concurrent;
using CamAI.EdgeBox.Models;
using MassTransit;
using Microsoft.Extensions.Options;
using Serilog;

namespace CamAI.EdgeBox.Services.AI;

public class ClassifierProcessor : IDisposable
{
    private bool disposed;

    private readonly ConcurrentBag<ClassifierItem> classifierItems = [];
    private readonly PeriodicTimer timer;
    private readonly IPublishEndpoint bus;
    private readonly ClassifierWatcher watcher;

    public ClassifierProcessor(IOptions<AiConfiguration> configuration, IPublishEndpoint bus)
    {
        watcher = new ClassifierWatcher(configuration);
        watcher.Notifier += ReceiveData;
        timer = new PeriodicTimer(TimeSpan.FromSeconds(configuration.Value.Classifier.Interval));
        this.bus = bus;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var items = classifierItems.ToArray();
            if (items.Length == 0)
                continue;
            classifierItems.Clear();

            var total = 0;
            var maxPhone = 0;
            var maxLaptop = 0;
            foreach (var item in items)
            {
                var data = item.Output.Select(x => x.Data).ToList();
                total += data.Count;
                var actionGroup = data.GroupBy(x => x.Label).ToList();
                maxPhone = Math.Max(maxPhone, actionGroup.Count(x => x.Key == ActionType.Phone));
                maxLaptop = Math.Max(maxLaptop, actionGroup.Count(x => x.Key == ActionType.Laptop));
            }

            // TODO [Duy]: test the accuracy of this
            total /= items.Length;
            var result = new List<ClassifierResult>
            {
                new() { ActionType = ActionType.Phone, Count = maxPhone },
                new() { ActionType = ActionType.Laptop, Count = maxLaptop },
                new() { ActionType = ActionType.Idle, Count = total - maxLaptop - maxPhone }
            };
            var countModel = new ClassifierModel
            {
                Time = DateTime.Now,
                Total = total,
                Results = result,
                ShopId = GlobalData.Shop!.Id
            };
            try
            {
                Log.Information("{Time}: {Total}", countModel.Time, countModel.Total);
                await bus.Publish(countModel, CancellationToken.None);
            }
            catch (Exception)
            {
                Console.WriteLine("Error publishing message");
            }
        }
    }

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        classifierItems.Add(new ClassifierItem(time, output));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;
        if (disposing)
        {
            timer.Dispose();
            watcher.Dispose();
        }
        disposed = true;
    }
}

public record ClassifierItem(int Time, List<ClassifierOutputModel> Output);
