using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public static class ShopService
{
    public static Shop UpsertShop(Shop shop)
    {
        ShopRepository.UpsertShop(shop);
        GlobalData.Shop = shop;
        return GlobalData.Shop;
    }
}
