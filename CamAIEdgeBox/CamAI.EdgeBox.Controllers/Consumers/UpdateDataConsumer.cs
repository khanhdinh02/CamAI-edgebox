using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using MassTransit;
using RabbitMQ.Client;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

// TODO [DUY]: replace template
// [Consumer("{brandName}.{shopName}", Constants.UpdateData, "{brandId}.{shopId}", ExchangeType.Topic)]
[Consumer("brand1.shop1", Constants.UpdateData, "brand1.shop1", ExchangeType.Topic)]
public class UpdateDataConsumer(
    BrandService brandService,
    ShopService shopService,
    EmployeeService employeeService
) : IConsumer<BrandUpdateMessage>, IConsumer<ShopUpdateMessage>, IConsumer<EmployeeUpdateMessage>
{
    public Task Consume(ConsumeContext<BrandUpdateMessage> context)
    {
        var brand = context.Message.ToBrand();
        brandService.UpsertBrand(brand);
        GlobalData.Brand = brand;
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<ShopUpdateMessage> context)
    {
        var shop = context.Message.ToShop();
        shopService.UpsertShop(shop);
        GlobalData.Shop = shop;
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<EmployeeUpdateMessage> context)
    {
        var employee = context.Message.ToEmployee();
        employeeService.UpsertEmployee(employee);
        GlobalData.Employees = employeeService.GetEmployee();
        return Task.CompletedTask;
    }
}
