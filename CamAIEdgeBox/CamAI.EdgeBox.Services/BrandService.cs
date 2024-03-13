using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class BrandService(UnitOfWork unitOfWork)
{
    public Brand? GetBrand()
    {
        return unitOfWork.Brands.GetAll(false).FirstOrDefault();
    }

    public Brand UpsertBrand(Brand brand)
    {
        var foundBrand = unitOfWork.Brands.GetAll(false).FirstOrDefault();
        if (foundBrand == null)
            // insert
            unitOfWork.Brands.Add(brand);
        else
        {
            // update
            brand.Id = foundBrand.Id;
            unitOfWork.Brands.Update(brand);
        }
        unitOfWork.Complete();
        GlobalData.Brand = brand;
        return brand;
    }
}
