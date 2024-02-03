using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopsController(ShopService shopService) : Controller
{
    [HttpGet]
    public Shop? GetShop()
    {
        return GlobalData.Shop;
    }

    [HttpPut]
    public Shop UpsertShop([FromBody] Shop shopDto)
    {
        var shop = shopService.UpsertShop(shopDto);
        return shop;
    }
}
