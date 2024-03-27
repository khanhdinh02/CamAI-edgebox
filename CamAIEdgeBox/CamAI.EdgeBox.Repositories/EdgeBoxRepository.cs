using System.Text.Json;
using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public static class EdgeBoxRepository
{
    private static readonly Mutex Mutex = new();

    public static DbEdgeBox? Get()
    {
        try
        {
            return JsonSerializer.Deserialize<DbEdgeBox>(File.ReadAllText(GetPath()));
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public static void UpsertEdgeBox(DbEdgeBox brand)
    {
        Mutex.WaitOne();
        File.WriteAllText(GetPath(), JsonSerializer.Serialize(brand));
        Mutex.ReleaseMutex();
    }

    private static string GetPath() =>
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            $"edgebox_{Environment.MachineName}.json"
        );
}
