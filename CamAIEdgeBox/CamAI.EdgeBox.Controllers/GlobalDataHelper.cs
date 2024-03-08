using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;

namespace CamAI.EdgeBox.Controllers;

public class GlobalDataHelper(
    BrandService brandService,
    ShopService shopService,
    EdgeBoxService edgeBoxService,
    CameraService cameraService
)
{
    public void GetData()
    {
        GlobalData.Shop = shopService.GetShop();
        GlobalData.Brand = brandService.GetBrand();
        GlobalData.Cameras = cameraService.GetCamera();
        GlobalData.EdgeBox = edgeBoxService.GetEdgeBox();
    }
}
