using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CamAI.EdgeBox.Services.Utils;

public static class NetworkUtil
{
    public static string GetLocalIpAddress()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        var localIp = endPoint.Address.ToString();

        return localIp;
    }

    public static string GetMacAddress()
    {
        var macAddr = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic =>
                nic.OperationalStatus == OperationalStatus.Up
                && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
            )
            .Select(nic => nic.GetPhysicalAddress())
            .FirstOrDefault();

        return macAddr == null
            ? ""
            : string.Join(":", macAddr.GetAddressBytes().Select(b => b.ToString("X2")));
    }

    public static string GetOsName()
    {
        if (Environment.OSVersion.Platform != PlatformID.Unix)
            return Environment.OSVersion.Platform.ToString();

        var osName = File.ReadLines("/etc/os-release")
            .FirstOrDefault(x => x.StartsWith("PRETTY_NAME"));
        return osName?.Split("=")[1].Trim('"') ?? "Linux";
    }
}
