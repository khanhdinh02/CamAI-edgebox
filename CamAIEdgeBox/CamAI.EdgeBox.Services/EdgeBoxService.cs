using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class EdgeBoxService(UnitOfWork unitOfWork)
{
    public DbEdgeBox? GetEdgeBox()
    {
        return unitOfWork.EdgeBoxes.GetAll().FirstOrDefault();
    }

    public DbEdgeBox UpsertEdgeBox(DbEdgeBox edgeBox)
    {
        // TODO: detach entity
        var foundEdgeBox = unitOfWork.EdgeBoxes.GetAll().FirstOrDefault();
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
