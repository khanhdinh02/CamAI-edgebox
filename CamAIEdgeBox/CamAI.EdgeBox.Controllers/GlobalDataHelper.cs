using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services;

namespace CamAI.EdgeBox.Controllers;

public static class GlobalDataHelper
{
    public static void GetData()
    {
        GlobalData.Shop = ShopRepository.Get();
        GlobalData.Brand = BrandRepository.Get();
        GlobalData.Cameras = CameraRepository.GetAll();
        GlobalData.EdgeBox = EdgeBoxRepository.Get();
    }
}
