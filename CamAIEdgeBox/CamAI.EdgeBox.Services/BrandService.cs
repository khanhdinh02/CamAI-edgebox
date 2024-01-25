using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class BrandService(UnitOfWork unitOfWork)
{
    public Brand GetBrand()
    {
        return unitOfWork.Brands.GetAll()[0];
    }

    public Brand UpsertBrand(Brand brand)
    {
        // TODO: detach entity
        var foundBrand = unitOfWork.Brands.GetAll().FirstOrDefault();
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
        return brand;
    }
}
