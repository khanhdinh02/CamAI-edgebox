using CamAI.EdgeBox.Services.AI;
using MassTransit;

namespace CamAI.EdgeBox.Services;

public class AIService(IPublishEndpoint bus, IServiceProvider provider)
{
    public void RunAI()
    {
        AiProcessManager.Run("Test", provider);
    }

    public void KillAI()
    {
        AiProcessManager.Kill("Test");
    }
}
