using System.Reflection;
using MassTransit;

namespace CamAI.EdgeBox.MassTransit;

public static class ReceiveEndpointConfigurator
{
    public static void ConfigureBus(
        this IReceiveConfigurator cfg,
        IRegistrationContext context,
        IList<Assembly> assemblies
    )
    {
        // Register topic consumer if bus is Azure ServiceBus
        if (cfg.GetType().IsAssignableTo(typeof(IRabbitMqBusFactoryConfigurator)))
        {
            // var topicConsumers = GetConsumers<TopicConsumerAttribute>(assemblies)
            //     .GroupBy(x => x.Name);
            // foreach (var topic in topicConsumers)
            //     RegisterTopicConsumer(
            //         (IServiceBusBusFactoryConfigurator)cfg,
            //         context,
            //         topic.Key,
            //         topic.Select(x => x.Type)
            //     );

            // Register queue consumer
            var queueConsumers = assemblies
                .GetConsumers<QueueConsumerAttribute>()
                .GroupBy(x => x.Name);
            foreach (var queue in queueConsumers)
                RegisterQueueConsumer(cfg, context, queue.Key, queue.Select(x => x.Type));
        }
        else if (cfg.GetType().IsAssignableTo(typeof(IInMemoryBusFactoryConfigurator)))
        {
            // This method is only for local and integration test to register queue (RabbitMQ) consumer
            var topicConsumers = assemblies.GetConsumers<ConsumerAttribute>().GroupBy(x => x.Name);
            foreach (var topic in topicConsumers)
                RegisterQueueConsumer(cfg, context, topic.Key, topic.Select(x => x.Type));
        }
    }

    private static IEnumerable<(Type Type, string Name)> GetConsumers<T>(
        this IList<Assembly> assemblies
    )
        where T : ConsumerAttribute
    {
        var types =
            from asm in assemblies
            from type in asm.ExportedTypes
            let attr = type.GetCustomAttribute<T>()
            where attr != null
            select (type, attr.Name);
        return types;
    }

    // private static void RegisterTopicConsumer(
    //     IServiceBusBusFactoryConfigurator cfg,
    //     IRegistrationContext context,
    //     string endpointName,
    //     IEnumerable<Type> consumers
    // )
    // {
    //     // TODO [NGUD, S13+]: Figure out a more consistent subscription name
    //     var random = new Random();
    //     cfg.SubscriptionEndpoint(
    //         $"{endpointName}_{random.Next()}",
    //         endpointName,
    //         e =>
    //         {
    //             e.RegisterConsumer(context, consumers);
    //         }
    //     );
    // }

    private static void RegisterQueueConsumer(
        IReceiveConfigurator cfg,
        IRegistrationContext context,
        string endpointName,
        IEnumerable<Type> consumers
    )
    {
        cfg.ReceiveEndpoint(endpointName, e => e.RegisterConsumer(context, consumers));
    }

    private static void RegisterConsumer(
        this IReceiveEndpointConfigurator e,
        IRegistrationContext context,
        IEnumerable<Type> consumers
    )
    {
        e.ConcurrentMessageLimit = 5;
        e.ConfigureConsumeTopology = false;

        foreach (var type in consumers)
            e.ConfigureConsumer(context, type);
    }
}
