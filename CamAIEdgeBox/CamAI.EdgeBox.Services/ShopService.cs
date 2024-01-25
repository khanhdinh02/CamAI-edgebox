using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class ShopService(UnitOfWork unitOfWork)
{
    public Shop GetShop()
    {
        return unitOfWork.Shops.GetAll()[0];
    }

    public Shop UpsertShop(Shop shop)
    {
        // TODO: detach entity
        var foundShop = unitOfWork.Shops.GetAll().FirstOrDefault();
        if (foundShop == null)
            // insert
            unitOfWork.Shops.Add(shop);
        else
        {
            // update
            shop.Id = foundShop.Id;
            unitOfWork.Shops.Update(shop);
        }
        unitOfWork.Complete();
        return shop;
    }
}
