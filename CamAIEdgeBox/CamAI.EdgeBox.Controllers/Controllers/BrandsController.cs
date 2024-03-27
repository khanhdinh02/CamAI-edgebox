using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BrandsController : Controller
{
    [HttpGet]
    public Brand? GetBrand()
    {
        return GlobalData.Brand;
    }

    [HttpPut]
    public Brand UpsertBrand([FromBody] Brand brandDto)
    {
        var brand = BrandService.UpsertBrand(brandDto);
        return brand;
    }
}
