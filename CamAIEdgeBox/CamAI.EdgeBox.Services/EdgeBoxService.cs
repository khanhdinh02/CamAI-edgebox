using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using Microsoft.Extensions.Logging;

namespace CamAI.EdgeBox.Services;

public class EdgeBoxService(ILogger<EdgeBoxService> logger)
{
    /// <summary>
    ///
    /// </summary>
    /// <returns>0 indicate edge box activation failed, otherwise successfully</returns>
    public int ActivateEdgeBox()
    {
        try
        {
            var edgeBox = GlobalData.EdgeBox!;
            if (edgeBox.EdgeBoxStatus == EdgeBoxStatus.Active)
                return 1;
            edgeBox.EdgeBoxStatus = EdgeBoxStatus.Active;
            EdgeBoxRepository.UpsertEdgeBox(edgeBox);
            return 1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return 0;
        }
    }

    public static DbEdgeBox UpsertEdgeBox(DbEdgeBox edgeBox)
    {
        EdgeBoxRepository.UpsertEdgeBox(edgeBox);
        GlobalData.EdgeBox = edgeBox;
        return edgeBox;
    }
}
