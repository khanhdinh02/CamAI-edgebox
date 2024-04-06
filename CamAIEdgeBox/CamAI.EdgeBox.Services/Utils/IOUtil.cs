namespace CamAI.EdgeBox.Services.Utils;

public static class IOUtil
{
    public static void DeleteDirectory(string path)
    {
        if (!Directory.Exists(path))
            return;

        // Enumerate all files and delete them first
        foreach (var file in Directory.EnumerateFiles(path))
            File.Delete(file);

        // Recursively delete subdirectories
        foreach (var directory in Directory.EnumerateDirectories(path))
            DeleteDirectory(directory);

        // Finally, delete the empty directory
        Directory.Delete(path, recursive: true);
    }
}
