using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using Serilog;

namespace CamAI.EdgeBox.Controllers;

public static class GlobalDataHelper
{
    public static void GetData()
    {
        Log.Information("Fetching local data");
        GlobalData.Shop = ShopRepository.Get();
        GlobalData.Brand = BrandRepository.Get();
        GlobalData.Cameras = GlobalData.Shop != null ? CameraRepository.GetAll() : [];
        GlobalData.EdgeBox = EdgeBoxRepository.Get();
    }
}
