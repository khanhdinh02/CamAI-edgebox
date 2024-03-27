using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using MassTransit;
using RabbitMQ.Client;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

[Consumer("{BrandName}.{ShopName}", Constants.UpdateData, "{BrandId}.{ShopId}", ExchangeType.Topic)]
public class UpdateDataConsumer : IConsumer<BrandUpdateMessage>, IConsumer<ShopUpdateMessage>
{
    public Task Consume(ConsumeContext<BrandUpdateMessage> context)
    {
        var brand = context.Message.ToBrand();
        BrandService.UpsertBrand(brand);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<ShopUpdateMessage> context)
    {
        var shop = context.Message.ToShop();
        ShopService.UpsertShop(shop);
        return Task.CompletedTask;
    }
}
