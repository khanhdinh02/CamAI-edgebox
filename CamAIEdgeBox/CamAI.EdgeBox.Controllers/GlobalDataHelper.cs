using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services;

namespace CamAI.EdgeBox.Controllers;

public class GlobalDataHelper(UnitOfWork unitOfWork)
{
    public void GetData()
    {
        GlobalData.Shop = unitOfWork.Shops.GetAll(false).FirstOrDefault();
        GlobalData.Brand = unitOfWork.Brands.GetAll(false).FirstOrDefault();
        GlobalData.Cameras = unitOfWork.Cameras.GetAll(false);
        GlobalData.EdgeBox = unitOfWork.EdgeBoxes.GetAll(false).FirstOrDefault();
    }
}
