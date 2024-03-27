using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public static class BrandService
{
    public static Brand UpsertBrand(Brand brand)
    {
        BrandRepository.UpsertBrand(brand);
        GlobalData.Brand = brand;
        return brand;
    }
}
