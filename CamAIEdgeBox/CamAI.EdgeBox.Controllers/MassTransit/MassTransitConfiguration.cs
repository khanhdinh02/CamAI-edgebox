using System.Reflection;
using CamAI.EdgeBox.Consumers;
using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.Models;
using MassTransit;
using Microsoft.Extensions.FileSystemGlobbing;

namespace CamAI.EdgeBox.MassTransit;

public static class MassTransitConfiguration
{
    public static IBusControl CreateSyncBusControl(
        this WebApplicationBuilder builder,
        UpdateDataConsumer updateDataConsumer,
        Guid edgeBoxId
    )
    {
        var settings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>()!;
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.SetupHost(settings);

            cfg.Send<RoutingKeyMessage>(configurator =>
                configurator.UseRoutingKeyFormatter(sendContext => sendContext.Message.RoutingKey)
            );

            // register sync data request message
            cfg.Message<InitializeRequest>(x =>
            {
                x.SetEntityName(
                    typeof(InitializeRequest).GetCustomAttribute<PublisherAttribute>()!.QueueName
                );
            });

            cfg.Message<ConfirmedEdgeBoxActivationMessage>(x =>
            {
                x.SetEntityName(
                    typeof(ConfirmedEdgeBoxActivationMessage)
                        .GetCustomAttribute<PublisherAttribute>()!
                        .QueueName
                );
            });

            cfg.Message<HealthCheckResponseMessage>(x =>
            {
                x.SetEntityName(
                    typeof(HealthCheckResponseMessage)
                        .GetCustomAttribute<PublisherAttribute>()!
                        .QueueName
                );
            });

            var consumer = typeof(UpdateDataConsumer).GetCustomAttribute<ConsumerAttribute>()!;
            cfg.AutoStart = true;
            cfg.ReceiveEndpoint(
                $"EdgeBox_{edgeBoxId:N}",
                e =>
                {
                    e.ConcurrentMessageLimit = 5;
                    e.ConfigureConsumeTopology = false;
                    e.DiscardFaultedMessages();
                    e.DiscardSkippedMessages();

                    ConsumerExtensions.Consumer(
                        e,
                        typeof(UpdateDataConsumer),
                        _ => updateDataConsumer
                    );
                    e.Bind(
                        consumer.ExchangeName,
                        x =>
                        {
                            x.ExchangeType = consumer.ExchangeType;
                            x.RoutingKey = edgeBoxId.ToString("N");
                        }
                    );
                }
            );
        });
        return busControl;
    }

    public static void ConfigureMassTransit(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
        var assemblies = GetLoadedAssemblies();

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumers(assemblies);

            if (settings!.HostName == "in-memory")
            {
                RegisterPublisherEndpoint(assemblies);
                x.UsingInMemory((context, cfg) => cfg.RegisterConsumer(context, assemblies));
            }
            else
                x.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        cfg.SetupHost(settings);

                        cfg.Send<RoutingKeyMessage>(configurator =>
                            configurator.UseRoutingKeyFormatter(sendContext =>
                                sendContext.Message.RoutingKey
                            )
                        );
                        cfg.RegisterPublisher(assemblies);
                        cfg.RegisterConsumer(context, assemblies);
                    }
                );
        });
    }

    private static void RegisterPublisherEndpoint(Assembly[] assemblies)
    {
        var mapMethod = typeof(EndpointConvention).GetMethod(
            nameof(EndpointConvention.Map),
            [typeof(Uri)]
        )!;
        var typesToPublish = GetEntityTypeAndBusEndpoint(assemblies);
        foreach (var (type, uri) in typesToPublish)
            mapMethod.MakeGenericMethod(type).Invoke(null, [uri]);
    }

    private static IEnumerable<(Type type, Uri Uri)> GetEntityTypeAndBusEndpoint(
        IEnumerable<Assembly> loadedAssemblies
    )
    {
        return from asm in loadedAssemblies
            from type in asm.ExportedTypes
            let attr = type.GetCustomAttribute<PublisherAttribute>()
            where attr != null
            select (type, attr.Uri);
    }

    private static void SetupHost(
        this IRabbitMqBusFactoryConfigurator cfg,
        RabbitMqConfiguration settings
    )
    {
        cfg.Host(
            settings.HostName,
            settings.Port,
            settings.VirtualHost,
            h =>
            {
                h.Username(settings.Username);
                h.Password(settings.Password);
            }
        );
    }

    private static Assembly[] GetLoadedAssemblies()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var matcher = new Matcher();
        matcher.AddIncludePatterns(["CamAI.*.dll"]);
        var assemblies = matcher.GetResultsInFullPath(path).Select(Assembly.LoadFrom);
        return assemblies.ToArray();
    }
}
