using CamAI.EdgeBox.Controllers.Models;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopsController : Controller
{
    [HttpGet]
    public ShopDto? GetShop()
    {
        var shop = GlobalData.Shop!;
        return new ShopDto
        {
            Name = shop.Name,
            Phone = shop.Phone,
            Address = shop.Address,
            OpenTime = shop.OpenTime,
            CloseTime = shop.CloseTime,
            IsShopOpen = AiService.IsShopOpen()
        };
    }

    [HttpPut]
    public Shop UpsertShop([FromBody] Shop shopDto)
    {
        var shop = ShopService.UpsertShop(shopDto);
        return shop;
    }
}
