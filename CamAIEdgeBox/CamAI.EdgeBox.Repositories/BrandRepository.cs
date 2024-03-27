using System.Text.Json;
using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public static class BrandRepository
{
    private static readonly Mutex Mutex = new();

    public static Brand? Get()
    {
        try
        {
            return JsonSerializer.Deserialize<Brand>(File.ReadAllText(GetPath()));
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public static void UpsertBrand(Brand brand)
    {
        Mutex.WaitOne();
        File.WriteAllText(GetPath(), JsonSerializer.Serialize(brand));
        Mutex.ReleaseMutex();
    }

    private static string GetPath() =>
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            $"brand_{Environment.MachineName}.json"
        );
}
