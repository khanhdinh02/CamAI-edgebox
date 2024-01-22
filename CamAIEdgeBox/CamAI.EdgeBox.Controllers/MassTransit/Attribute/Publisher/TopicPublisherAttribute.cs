namespace CamAI.EdgeBox.MassTransit;

public class TopicPublisherAttribute(string template) : PublisherAttribute(template)
{
    public override Uri Uri => new("topic:" + Name);
}
