using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Serilog;

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

    public static string GetSerialNumber()
    {
        try
        {
            var serialNumber = File.ReadAllText("/sys/firmware/devicetree/base/serial-number");
            if (!string.IsNullOrEmpty(serialNumber))
                return serialNumber.Replace("\0", string.Empty).Trim();
        }
        catch { }

        try
        {
            Console.WriteLine(
                "Path {0}",
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "serial_number"
                )
            );
            var serialNumber = File.ReadAllText(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "serial_number"
                )
            );
            if (!string.IsNullOrEmpty(serialNumber))
                return serialNumber.Replace("\0", string.Empty).Trim();
        }
        catch { }
        return "";
        // try
        // {
        //     var proc = new Process
        //     {
        //         StartInfo = new ProcessStartInfo
        //         {
        //             FileName = "dmidecode",
        //             Arguments = "-t system",
        //             UseShellExecute = false,
        //             RedirectStandardOutput = true,
        //             CreateNoWindow = true
        //         }
        //     };
        //     proc.Start();
        //     while (!proc.StandardOutput.EndOfStream)
        //     {
        //         var line = proc.StandardOutput.ReadLine();
        //         if (line != null)
        //             return line;
        //     }
        // }
        // catch { }
        //
        // Log.Fatal("InformationCannot get edge box serial number");
        // Environment.Exit(60);
        // return "";
    }
}
