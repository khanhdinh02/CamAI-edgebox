using CamAI.EdgeBox.Controllers.Models;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.Utils;
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

        return Ok(new { Token = Hasher.Hash("yes") });
    }

    [HttpPost("password")]
    public IActionResult ChangePassword(ChangePasswordDto dto)
    {
        if (AuthService.ChangePassword(dto.OldPassword, dto.NewPassword))
            return Ok();
        return Unauthorized("Wrong old password");
    }
}
