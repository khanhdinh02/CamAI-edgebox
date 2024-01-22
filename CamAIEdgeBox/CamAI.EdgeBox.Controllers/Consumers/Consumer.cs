using CamAI.EdgeBox.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Consumers;

[QueueConsumer("test")]
public class TestConsumer : IConsumer<Test>
{
    public Task Consume(ConsumeContext<Test> context)
    {
        return Task.CompletedTask;
    }
}
