namespace CamAI.EdgeBox.MassTransit;

public class QueuePublisherAttribute(string template) : PublisherAttribute(template)
{
  public override Uri Uri => new("queue:" + Name);
}
