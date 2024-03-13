using System.Collections.Concurrent;
using CamAI.EdgeBox.Models;
using MassTransit;
using Serilog;

namespace CamAI.EdgeBox.Services.AI;

public class HumanCountProcessor : IDisposable
{
    private bool disposed;

    private readonly ConcurrentBag<HumanCountItem> humanCountItems = [];
    private readonly PeriodicTimer timer;
    private readonly IPublishEndpoint bus;

    public HumanCountProcessor(
        ClassifierWatcher watcher,
        HumanCountConfiguration humanCountConfiguration,
        IPublishEndpoint bus
    )
    {
        watcher.Notifier += ReceiveData;
        timer = new PeriodicTimer(TimeSpan.FromSeconds(humanCountConfiguration.Interval));
        this.bus = bus;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var items = humanCountItems.ToArray();
            if (items.Length == 0)
                continue;
            humanCountItems.Clear();

            var total = items
                .Select(item => item.Output.Select(x => x.Data).ToList())
                .Select(data => data.Count)
                .Average();

            var countModel = new HumanCountModel
            {
                Time = DateTime.Now,
                Total = Convert.ToInt32(total),
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
        humanCountItems.Add(new HumanCountItem(time, output));
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
            timer.Dispose();
        disposed = true;
    }
}

public record HumanCountItem(int Time, List<ClassifierOutputModel> Output);
