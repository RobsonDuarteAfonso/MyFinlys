using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinlys.Api.Services;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;

namespace MyFinlys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtAuthService _auth;
    private readonly IUserService _userService;

    public AuthController(JwtAuthService auth, IUserService userService)
    {
        _auth = auth;
        _userService = userService;
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var token = await _auth.GenerateTokenAsync(dto);
        return token is null
            ? Unauthorized("Credenciais inválidas.")
            : Ok(token);
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] UserCreateDto dto)
    {
        var existing = await _userService.GetByEmailAsync(dto.Email);
        if (existing is not null)
            return BadRequest("E-mail já cadastrado.");

        await _userService.CreateAsync(dto.Name, dto.Email, dto.Password);

        // Auto-login after register
        var token = await _auth.GenerateTokenAsync(new LoginRequestDto 
        { 
            Email = dto.Email, 
            Password = dto.Password 
        });

        return Ok(token);
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
