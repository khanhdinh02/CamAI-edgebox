using MassTransit;
using Serilog;

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

        model.Evidences.RemoveAll(x => !File.Exists(x.Path));
        foreach (var evidence in model.Evidences.Where(x => !x.IsSent))
            evidences.Add(
                new Evidence
                {
                    EvidenceType = EvidenceType.Image,
                    CameraId = evidence.CameraId,
                    Content = await File.ReadAllBytesAsync(evidence.Path, cancellationToken)
                }
            );

        var incident = new Incident
        {
            Id = model.Id,
            AiId = model.AiId,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            IncidentType = model.Type,
            Evidences = evidences
        };

        Log.Information(
            "Send new incident {Type}, incident id {IncidentId}",
            incident.IncidentType,
            incident.Id
        );

        try
        {
            await bus.Publish(incident, cancellationToken);
            foreach (var evidence in model.Evidences.Where(x => !x.IsSent))
                evidence.IsSent = true;
        }
        catch (Exception ex)
        {
            Log.Information("oh no, send incident has exception, {Ex}", ex);
        }
    }

    public static void CaptureEvidence(
        this AiProcessWrapper.AiProcessUtil util,
        AiIncidentModel model
    )
    {
        Log.Information(
            "Capture new evidence for model {Type}, AI id {AiId}",
            model.Type,
            model.AiId
        );
        var captureName = util.CaptureFrame(model.NewEvidenceName());
        model.Evidences.Add(
            new CalculationEvidence { Path = captureName, CameraId = util.CameraId }
        );
    }

    public static bool ShouldBeSend(this AiIncidentModel model)
    {
        // send if there is new evidence or end of incident
        var evidence = model.Evidences.Where(x => !x.IsSent).ToList();
        return (evidence.Count != 0 && evidence.Max(x => x.Time).AddSeconds(5) < DateTime.Now)
            || model.EndTime != null;
    }

    public static void CleanUpEvidence(this AiIncidentModel model)
    {
        Log.Information(
            "Cleaning up evidence for type {Type}, AiId {AiId}",
            model.Type,
            model.AiId
        );
        foreach (var evidence in model.Evidences)
            File.Delete(evidence.Path);
    }
}
