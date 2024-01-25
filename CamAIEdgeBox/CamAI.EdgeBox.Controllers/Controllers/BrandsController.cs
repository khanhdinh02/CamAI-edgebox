using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BrandsController(BrandService brandService) : Controller
{
    [HttpGet]
    public Brand GetBrand()
    {
        return brandService.GetBrand();
    }

    [HttpPut]
    public Brand UpsertBrand([FromBody] Brand brand)
    {
        return brandService.UpsertBrand(brand);
    }
}
