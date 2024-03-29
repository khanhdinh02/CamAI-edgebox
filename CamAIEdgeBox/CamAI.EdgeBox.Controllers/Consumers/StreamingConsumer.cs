using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.Streaming;
using MassTransit;
using RabbitMQ.Client;
using Constants = CamAI.EdgeBox.Services.MassTransit.Constants;

namespace CamAI.EdgeBox.Consumers;

[Consumer("{EdgeBoxId}_Streaming", Constants.Streaming, "{EdgeBoxId}", ExchangeType.Direct)]
public class StreamingConsumer(CameraService cameraService) : IConsumer<StreamingMessage>
{
    public Task Consume(ConsumeContext<StreamingMessage> context)
    {
        var message = context.Message;
        StartStreaming(message);
        return Task.CompletedTask;
    }

    private void StartStreaming(StreamingMessage message)
    {
        var camera = cameraService.GetCamera(message.CameraId);
        StreamingProcessManager.RunEncoder(camera, message.HttpRelayUri!);
    }

    private void EndStreaming(StreamingMessage message)
    {
        var camera = cameraService.GetCamera(message.CameraId);
        StreamingProcessManager.Kill(camera);
    }
}
