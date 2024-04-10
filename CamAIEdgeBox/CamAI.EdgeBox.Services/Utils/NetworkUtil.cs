using System.Net;
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
}
