namespace CamAI.EdgeBox.MassTransit;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceBusEndpointAttribute : Attribute
{
    protected readonly string Template;

    protected ServiceBusEndpointAttribute(string template)
    {
        Template = template;
    }

    public virtual string Name => Template;
}
