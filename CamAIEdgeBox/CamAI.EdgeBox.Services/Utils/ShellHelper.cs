using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CamAI.EdgeBox.Services.Utils;

public static class ShellHelper
{
    /// <summary>
    /// ref <see cref="https://jackma.com/2019/04/20/execute-a-bash-script-via-c-net-core/"/>
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static Task<int> Bash(this string cmd, ILogger logger)
    {
        var source = new TaskCompletionSource<int>();
        var escapedArgs = cmd.Replace("\"", "\\\"");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };
        process.Exited += async (sender, args) =>
        {
            logger.LogWarning(await process.StandardError.ReadToEndAsync());
            logger.LogInformation(await process.StandardOutput.ReadToEndAsync());
            if (process.ExitCode == 0)
            {
                source.SetResult(0);
            }
            else
            {
                source.SetException(
                    new Exception($"Command `{cmd}` failed with exit code `{process.ExitCode}`")
                );
            }

            process.Dispose();
        };

        try
        {
            process.Start();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Command {} failed", cmd);
            source.SetException(e);
        }

        return source.Task;
    }

    public static async Task WindowsPrompt(this string command, ILogger logger)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };

        process.Start();
        process.StandardInput.WriteLine(command);
        process.StandardInput.Flush();
        process.StandardInput.Close();
        process.WaitForExit();
        logger.LogWarning(await process.StandardError.ReadToEndAsync());
        logger.LogInformation(await process.StandardOutput.ReadToEndAsync());
    }

    public static async Task LinuxCmd(this string cmd, ILogger logger)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c {cmd}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }

        };
        process.Start();
        logger.LogWarning(await process.StandardError.ReadToEndAsync());
        logger.LogInformation(await process.StandardOutput.ReadToEndAsync());
        await process.WaitForExitAsync();
    }

}
