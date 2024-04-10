using CamAI.EdgeBox.Controllers.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    [HttpPost("login")]
    public IActionResult Login(LoginDto loginDto)
    {
        if (!AuthService.Login(loginDto.Username, loginDto.Password))
            return Unauthorized();

        HttpContext.Session.SetString("ID", "something");
        return Ok();
    }

    [HttpPost("password")]
    public IActionResult ChangePassword(ChangePasswordDto dto)
    {
        if (AuthService.ChangePassword(dto.OldPassword, dto.NewPassword))
            return Ok();
        return Unauthorized("Wrong old password");
    }
}
