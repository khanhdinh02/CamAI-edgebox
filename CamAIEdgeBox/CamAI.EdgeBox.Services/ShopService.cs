using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class ShopService(UnitOfWork unitOfWork)
{
    public Shop? GetShop()
    {
        return unitOfWork.Shops.GetAll(false).FirstOrDefault();
    }

    public Shop UpsertShop(Shop shop)
    {
        var foundShop = unitOfWork.Shops.GetAll(false).FirstOrDefault();
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
        unitOfWork.Detach(shop);
        GlobalData.Shop = unitOfWork.Shops.GetAll(false).First();
        return GlobalData.Shop;
    }
}
