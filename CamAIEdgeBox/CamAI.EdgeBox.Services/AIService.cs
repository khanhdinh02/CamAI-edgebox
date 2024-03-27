using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.AI;

namespace CamAI.EdgeBox.Services;

public class AiService(IServiceProvider provider)
{
    private Timer timer;

    private void SetUpTimer(TimeOnly runAtTime, Action action)
    {
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        var nextRunTime = runAtTime.ToTimeSpan();
        if (currentTime > runAtTime)
            nextRunTime += TimeSpan.FromDays(1);

        timer = new Timer(
            _ => action.Invoke(),
            null,
            nextRunTime - currentTime.ToTimeSpan(),
            Timeout.InfiniteTimeSpan
        );
    }

    public void RunAi()
    {
        var shop = GlobalData.Shop!;
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        if (shop.OpenTime < currentTime && currentTime < shop.CloseTime)
            StartAiProcess();
        else
            KillAi();
    }

    private void StartAiProcess()
    {
        var shop = GlobalData.Shop!;
        foreach (var camera in GlobalData.Cameras)
            AiProcessManager.Run(camera, provider);
        SetUpTimer(shop.CloseTime, KillAi);
    }

    private void KillAi()
    {
        var shop = GlobalData.Shop!;
        AiProcessManager.KillAll();
        SetUpTimer(shop.OpenTime, StartAiProcess);
    }
}
