using System.Net;
using System.Net.Sockets;

namespace CamAI.EdgeBox.Services.Utils;

public static class NetworkUtil
{
    public static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
