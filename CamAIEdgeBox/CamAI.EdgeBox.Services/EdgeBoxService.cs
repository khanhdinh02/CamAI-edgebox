using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using Microsoft.Extensions.Logging;

namespace CamAI.EdgeBox.Services;

public class EdgeBoxService(UnitOfWork unitOfWork, ILogger<EdgeBoxService> logger)
{
    public DbEdgeBox? GetEdgeBox()
    {
        return unitOfWork.EdgeBoxes.GetAll().FirstOrDefault();
    }

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
            unitOfWork.EdgeBoxes.Update(edgeBox);
            return unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return 0;
        }
    }

    public DbEdgeBox UpsertEdgeBox(DbEdgeBox edgeBox)
    {
        var foundEdgeBox = unitOfWork.EdgeBoxes.GetAll(false).FirstOrDefault();
        if (foundEdgeBox == null)
            // insert
            unitOfWork.EdgeBoxes.Add(edgeBox);
        else
        {
            // update
            edgeBox.Id = foundEdgeBox.Id;
            unitOfWork.EdgeBoxes.Update(edgeBox);
        }
        unitOfWork.Complete();
        GlobalData.EdgeBox = edgeBox;
        return edgeBox;
    }
}
