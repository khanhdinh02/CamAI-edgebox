using System.Collections.Concurrent;
using MassTransit;
using Microsoft.Extensions.Options;
using CamAI.EdgeBox.Services.Utils;
using static CamAI.EdgeBox.Services.AI.EnumConversion;

namespace CamAI.EdgeBox.Services.AI;

public class ClassifierProcessor
{
    private readonly ConcurrentBag<ClassifierItem> classifierItems = [];
    private readonly PeriodicTimer timer;
    private readonly IPublishEndpoint bus;

    public ClassifierProcessor(IOptions<AiConfiguration> configuration, IPublishEndpoint bus)
    {
        var watcher = new ClassifierWatcher(configuration);
        watcher.Notifier += ReceiveData;
        timer = new PeriodicTimer(TimeSpan.FromSeconds(configuration.Value.Classifier.Interval));
        this.bus = bus;
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var items = classifierItems.ToArray();
            classifierItems.Clear();

            var actionDict = new Dictionary<ActionType, int>();
            foreach (var item in items)
            {
                var data = item.Output.Select(x => x.Data).GroupBy(x => x.Label);
                foreach (var group in data)
                    actionDict.SetOrIncrease(ActionTypes[group.Key], group.Count());
            }
            var countModel = new ClassifierModel
            {
                Time = DateTime.Now,
                Total = actionDict.Select(x => x.Value).Sum()/items.Length,
                Results = actionDict.Select(x => new ClassifierResult
                {
                    // TODO [Duy]: process the phone and laptop classification
                    ActionType = x.Key,
                    Count = x.Value/items.Length
                }).ToList()
                // TODO [Duy]: add shop id from global data after merge
            };
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

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        classifierItems.Add(new ClassifierItem(time, output));
    }
}

public record ClassifierItem(int Time, List<ClassifierOutputModel> Output);
