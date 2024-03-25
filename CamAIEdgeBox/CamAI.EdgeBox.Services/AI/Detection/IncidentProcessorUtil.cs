using CamAI.EdgeBox.Services.AI.Uniform;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;

namespace CamAI.EdgeBox.Services.AI.Detection;

public static class IncidentProcessorUtil
{
    public static async Task SendIncident(
        this IPublishEndpoint bus,
        AiIncidentModel model,
        CancellationToken cancellationToken = default
    )
    {
        var evidences = new List<Evidence>();
        foreach (var evidence in model.Evidences.Where(x => !x.IsSent))
        {
            evidence.IsSent = true;
            evidences.Add(
                new Evidence
                {
                    EvidenceType = EvidenceType.Image,
                    CameraId = evidence.CameraId,
                    FilePath = evidence.Path
                }
            );
        }
        var incidentType = model switch
        {
            PhoneModel => IncidentType.Phone,
            UniformModel => IncidentType.Uniform,
            InteractionModel => IncidentType.Interaction,
            _ => IncidentType.Phone
        };

        var incident = new Incident
        {
            Id = model.Id,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            IncidentType = incidentType,
            Evidences = evidences
        };

        await bus.Publish(incident, cancellationToken);
    }

    public static void CaptureEvidence(this RtspExtension rtsp, AiIncidentModel model)
    {
        var captureName = rtsp.CaptureFrame(model.NewEvidenceName());
        model.Evidences.Add(new CalculationEvidence { Path = captureName });
    }

    public static bool ShouldBeSend(this AiIncidentModel model) =>
        // send if there is new evidence or end of incident
        model.Evidences.Exists(x => !x.IsSent)
        || model.EndTime != null;
}
