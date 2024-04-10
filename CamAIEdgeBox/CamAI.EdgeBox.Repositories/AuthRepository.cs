namespace CamAI.EdgeBox.Repositories;

public static class AuthRepository
{
    public static string GetPassword()
    {
        var path = GetPath();
        return File.Exists(path) ? File.ReadAllText(path) : "";
    }

    public static void UpdatePassword(string password) => File.WriteAllText(GetPath(), password);

    private static string GetPath() =>
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            $"password_{Environment.MachineName}"
        );
}
