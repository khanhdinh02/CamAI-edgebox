using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopsController(ShopService shopService) : Controller
{
    [HttpGet]
    public Shop GetShop()
    {
        return shopService.GetShop();
    }

    [HttpPut]
    public Shop UpsertShop([FromBody] Shop shop)
    {
        return shopService.UpsertShop(shop);
    }
}
