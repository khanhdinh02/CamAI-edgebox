namespace CamAI.EdgeBox.Services.Streaming;

public class StreamingConfiguration
{
    public static string Section => "Streaming";
    public string FFMpegPath { get; set; } = null!;
}
