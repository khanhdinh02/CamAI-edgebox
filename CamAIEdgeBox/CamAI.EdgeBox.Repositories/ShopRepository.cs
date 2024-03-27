using System.Text.Json;
using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public static class ShopRepository
{
    private static readonly Mutex Mutex = new();

    public static Shop? Get()
    {
        try
        {
            return JsonSerializer.Deserialize<Shop>(File.ReadAllText(GetPath()));
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public static void UpsertShop(Shop shop)
    {
        Mutex.WaitOne();
        File.WriteAllText(GetPath(), JsonSerializer.Serialize(shop));
        Mutex.ReleaseMutex();
    }

    private static string GetPath() =>
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            $"shop_{Environment.MachineName}.json"
        );
}
