using System.Collections.Concurrent;
using MassTransit;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class HumanCountProcessor
{
    private readonly ConcurrentBag<HumanCountItem> items = [];
    private readonly PeriodicTimer timer;
    private readonly IPublishEndpoint bus;

    public HumanCountProcessor(IOptions<AiConfiguration> configuration, IPublishEndpoint bus)
    {
        var watcher = new HumanCountWatcher(configuration);
        watcher.Notifier += ReceiveData;
        timer = new PeriodicTimer(TimeSpan.FromSeconds(configuration.Value.HumanCount.Interval));
        this.bus = bus;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var count = items.ToArray();
            items.Clear();
            var countModel = new HumanCountModel
            {
                Time = DateTime.Now,
                Count = (int)count.Select(x => x.Count).Average(),
            };
            // TODO [Duy]: handle AI output here such as output fluctuation,...
            Console.WriteLine("Avg: {0}: {1}", countModel.Time, countModel.Count);
            try
            {
                await bus.Publish(countModel, CancellationToken.None);
            }
            catch (Exception)
            {
                Console.WriteLine("Error publishing message");
            }
        }
    }

    private void ReceiveData(int time, int count)
    {
        items.Add(new HumanCountItem(time, count));
        Console.WriteLine($"{time}: {count}");
    }
}

public record HumanCountItem(int Time, int Count);
