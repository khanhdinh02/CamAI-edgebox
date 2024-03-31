using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.AI;
using Serilog;

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
        // check for edge box activation
        if (GlobalData.EdgeBox?.EdgeBoxStatus != EdgeBoxStatus.Active)
            return;
        if (IsShopOpen())
        {
            Log.Information("Running AI for shop {ShopId}", GlobalData.Shop!.Id);
            StartAllAiProcess();
        }
        else
            KillAi();
    }

    public void RunAi(Camera camera)
    {
        // check for edge box activation and camera will run ai
        if (GlobalData.EdgeBox?.EdgeBoxStatus != EdgeBoxStatus.Active || !camera.WillRunAI)
            return;

        if (IsShopOpen())
        {
            Log.Information("Running AI for a camera {CameraId}", camera.Id);
            StartAiProcess(camera);
        }
    }

    private void StartAiProcess(Camera camera)
    {
        Log.Information("Start AI Process");
        AiProcessManager.Run(camera, provider);
    }

    private void StartAllAiProcess()
    {
        Log.Information("Start all AI Process");
        var shop = GlobalData.Shop!;
        foreach (var camera in GlobalData.Cameras.Where(x => x.WillRunAI))
            AiProcessManager.Run(camera, provider);
        SetUpTimer(shop.CloseTime, KillAi);
    }

    private void KillAi()
    {
        Log.Information("Kill AI Process");
        var shop = GlobalData.Shop!;
        AiProcessManager.KillAll();
        SetUpTimer(shop.OpenTime, StartAllAiProcess);
    }

    private static bool IsShopOpen()
    {
        var shop = GlobalData.Shop!;
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);

        Log.Information(
            "Shop open time {OpenTime}, close time {CloseTime}. Current time {Now}",
            shop.OpenTime,
            shop.CloseTime,
            currentTime
        );

        return shop.OpenTime < currentTime && currentTime < shop.CloseTime;
    }
}
