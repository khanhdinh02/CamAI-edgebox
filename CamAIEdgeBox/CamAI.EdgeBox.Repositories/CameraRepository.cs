using System.Text.Json;
using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public static class CameraRepository
{
    private static readonly Mutex Mutex = new();

    public static Camera? GetById(Guid id) => GetAll().Find(x => x.Id == id);

    public static List<Camera> GetAll()
    {
        return GlobalData.Cameras;
    }

    public static void UpsertCamera(Camera camera)
    {
        Mutex.WaitOne();
        var cameras = GetAll().Where(x => x.Id != camera.Id).ToList();
        cameras.Add(camera);
        GlobalData.Cameras = cameras;
        Mutex.ReleaseMutex();
    }

    public static void DeleteCamera(Guid id)
    {
        Mutex.WaitOne();
        var cameras = GetAll().Where(x => x.Id != id);
        File.WriteAllText(GetPath(), JsonSerializer.Serialize(cameras));
        Mutex.ReleaseMutex();
    }

    private static string GetPath() =>
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            $"camera_{Environment.MachineName}.json"
        );
}
