using CamAI.EdgeBox.Services.AI;

namespace CamAI.EdgeBox.Services;

public class AIService(IServiceProvider provider)
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
