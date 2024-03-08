using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using MassTransit;
using RabbitMQ.Client;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

[Consumer("{BrandName}.{ShopName}", Constants.UpdateData, "{BrandId}.{ShopId}", ExchangeType.Topic)]
public class UpdateDataConsumer(BrandService brandService, ShopService shopService)
    : IConsumer<BrandUpdateMessage>,
        IConsumer<ShopUpdateMessage>
{
    private static readonly Mutex Mutex = new();

    public Task Consume(ConsumeContext<BrandUpdateMessage> context)
    {
        var brand = context.Message.ToBrand();
        Mutex.WaitOne();
        brandService.UpsertBrand(brand);
        Mutex.ReleaseMutex();
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<ShopUpdateMessage> context)
    {
        var shop = context.Message.ToShop();
        Mutex.WaitOne();
        shopService.UpsertShop(shop);
        Mutex.ReleaseMutex();
        return Task.CompletedTask;
    }
}
