using System.Text;
using System.Web;
using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.MassTransit;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class MessageQueueEndpointAttribute(string queueName) : Attribute
{
    protected readonly string Template = queueName;
    public abstract string QueueName { get; }

    protected static string FormatTemplate(string template)
    {
        var sb = new StringBuilder(template);
        sb.Replace("{MachineName}", Environment.MachineName);
        sb.Replace("{BrandName}", GlobalData.Brand?.Name);
        sb.Replace("{ShopName}", GlobalData.Shop?.Name);
        sb.Replace("{BrandId}", GlobalData.Brand?.Id.ToString("N"));
        sb.Replace("{ShopId}", GlobalData.Shop?.Id.ToString("N"));
        sb.Replace("{EdgeBoxId}", GlobalData.EdgeBox?.Id.ToString("N"));
        sb.Replace(" ", "");
        return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
