using System;

namespace CamAI.EdgeBox.MassTransit;

public abstract class PublisherAttribute : ServiceBusEndpointAttribute
{
  protected PublisherAttribute(string template)
    : base(template) { }

  public abstract Uri Uri { get; }
}
