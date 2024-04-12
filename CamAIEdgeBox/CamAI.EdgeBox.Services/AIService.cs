using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.AI;
using CamAI.EdgeBox.Services.Utils;
using Serilog;

namespace CamAI.EdgeBox.Services;

public class AiService(IServiceProvider provider)
{
    private readonly System.Timers.Timer timer = new();

    private void SetUpTimer(TimeOnly runAtTime, Action action)
    {
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        var nextRunTime = runAtTime.ToTimeSpan();
        if (currentTime > runAtTime)
            nextRunTime += TimeSpan.FromDays(1);

        timer.Interval = (nextRunTime - currentTime.ToTimeSpan()).TotalMilliseconds;
        Log.Information(
            "Setup timer for next event, run at {RunAtTime}, wait for {Minute} minutes",
            nextRunTime,
            timer.Interval
        );
        timer.Elapsed += (_, _) =>
        {
            Log.Information("Timer elapsed at {CurrentTime}", TimeOnly.FromDateTime(DateTime.Now));
            action();
        };
        timer.AutoReset = false;
        timer.Start();
    }

    public void RunAi()
    {
        // check for edge box activation
        if (GlobalData.EdgeBox?.EdgeBoxStatus != EdgeBoxStatus.Active)
        {
            Log.Information("Edge box is not activated yet");
            return;
        }

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
        if (GlobalData.EdgeBox!.EdgeBoxStatus != EdgeBoxStatus.Active)
        {
            Log.Information("Edge box is not activated yet");
            return;
        }

        if (!camera.CanRunAI())
        {
            Log.Information(
                "Camera status {Status}, will run AI {WillRunAI}. Either of them is not satisfied",
                camera.Status,
                camera.WillRunAI
            );

            return;
        }

        if (IsShopOpen())
        {
            Log.Information("Running AI for a camera {CameraId}", camera.Id);
            StartAiProcess(camera);
        }
        else
        {
            Log.Information(
                "Shop is not open yet, cannot run AI process for camera {CameraId}",
                camera.Id
            );
        }
    }

    public (int NumOfExpectedAI, int NumOfRunningAI) GetRunningAIStatus()
    {
        return (GlobalData.Cameras.Count(x => x.CanRunAI()), AiProcessManager.NumOfRunningProcess);
    }

    public void KillAll()
    {
        AiProcessManager.KillAll();
    }

    public void DisableAi()
    {
        AiProcessManager.KillAll();
        timer.Stop();
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
        foreach (
            var camera in GlobalData.Cameras.Where(x =>
                x is { WillRunAI: true, Status: CameraStatus.Connected }
            )
        )
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

        if (shop.OpenTime == shop.CloseTime)
        {
            Log.Information(
                "Shop open time and close time are equal {OpenTime}, running AI all the time. Current time {Now}",
                shop.OpenTime,
                currentTime
            );
            return true;
        }

        Log.Information(
            "Shop open time {OpenTime}, close time {CloseTime}. Current time {Now}",
            shop.OpenTime,
            shop.CloseTime,
            currentTime
        );

        if (shop.OpenTime < shop.CloseTime)
            return shop.OpenTime < currentTime && currentTime < shop.CloseTime;
        return !(shop.OpenTime > currentTime && currentTime > shop.CloseTime);
    }
}
