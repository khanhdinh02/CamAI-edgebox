using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Controllers;

public static class GlobalDataHelper
{
    public static void GetData()
    {
        Console.WriteLine(
            "Fetching local data from: {0}",
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        );

        GlobalData.Shop = ShopRepository.Get();
        GlobalData.Brand = BrandRepository.Get();
        GlobalData.Cameras = GlobalData.Shop != null ? CameraRepository.GetAll() : [];
        GlobalData.EdgeBox = EdgeBoxRepository.Get();
    }
}
