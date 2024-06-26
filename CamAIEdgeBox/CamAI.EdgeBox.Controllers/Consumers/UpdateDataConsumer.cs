﻿using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using MassTransit;
using RabbitMQ.Client;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

[Consumer("{BrandId}.{ShopId}", Constants.UpdateData, "{BrandId}.{ShopId}", ExchangeType.Topic)]
public class UpdateDataConsumer(AiService aiService)
    : IConsumer<BrandUpdateMessage>,
        IConsumer<ShopUpdateMessage>,
        IConsumer<CameraUpdateMessage>,
        IConsumer<EdgeBoxUpdateMessage>
{
    public Task Consume(ConsumeContext<BrandUpdateMessage> context)
    {
        var brand = context.Message.ToBrand();
        GlobalData.Brand = brand;
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<ShopUpdateMessage> context)
    {
        var shop = context.Message.ToShop();
        GlobalData.Shop = shop;
        aiService.RunAi();
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<CameraUpdateMessage> context)
    {
        StaticCameraService.UpsertCameraFromServerData(context.Message.Cameras);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<EdgeBoxUpdateMessage> context)
    {
        var message = context.Message;
        GlobalData.EdgeBox = new DbEdgeBox
        {
            Name = message.Name,
            Model = message.Model,
            SerialNumber = message.SerialNumber,
            EdgeBoxStatus =
                message.ActivationStatus == EdgeBoxActivationStatus.Activated
                    ? EdgeBoxStatus.Active
                    : EdgeBoxStatus.Inactive
        };
        Console.WriteLine("Max number of running AI {0}", message.MaxNumberOfRunningAi);
        GlobalData.MaxNumberOfRunningAi = message.MaxNumberOfRunningAi;
        return Task.CompletedTask;
    }
}
