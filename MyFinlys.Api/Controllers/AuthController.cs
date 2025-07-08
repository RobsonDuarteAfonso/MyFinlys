using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinlys.Api.Services;
using MyFinlys.Application.DTOs;

namespace MyFinlys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtAuthService _auth;

    public AuthController(JwtAuthService auth) => _auth = auth;

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var token = await _auth.GenerateTokenAsync(dto);
        return token is null
            ? Unauthorized("Credenciais inválidas.")
            : Ok(token);
    }

    [HttpPut("change-password"), Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var changed = await _auth.ChangePasswordAsync(email, dto);
        return changed ? NoContent() : BadRequest("Senha atual incorreta.");
    }
}
