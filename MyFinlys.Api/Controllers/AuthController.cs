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
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public AuthController(
        JwtAuthService auth,
        IUserService userService,
        IEmailService emailService,
        IConfiguration config)
    {
        _auth = auth;
        _userService = userService;
        _emailService = emailService;
        _config = config;
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var token = await _auth.GenerateTokenAsync(dto);
        return token is null
            ? Unauthorized(new { message = "Invalid email or password." })
            : Ok(token);
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] UserCreateDto dto)
    {
        var existing = await _userService.GetByEmailAsync(dto.Email);
        if (existing is not null)
            return BadRequest(new { message = "E-mail already registered." });

        await _userService.CreateAsync(dto.Name, dto.Email, dto.Password, dto.Avatar);

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
        return changed ? NoContent() : BadRequest(new { message = "Current password is incorrect." });
    }

    [HttpPost("forgot-password"), AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        // Always return 200 to avoid user enumeration attacks
        var user = await _userService.GetByEmailAsync(dto.Email);
        if (user is null) return Ok(new { message = "If the email is registered, you will receive reset instructions." });

        var token = Guid.NewGuid().ToString("N");
        var expiry = DateTime.UtcNow.AddHours(1);

        await _userService.SetPasswordResetTokenAsync(dto.Email, token, expiry);

        var frontendUrl = _config["AppSettings:FrontendUrl"] ?? "http://localhost:4200";
        var resetLink = $"{frontendUrl}/reset-password?token={token}";

        await _emailService.SendPasswordResetEmailAsync(user.Email, user.Name, resetLink);

        return Ok(new { message = "If the email is registered, you will receive reset instructions." });
    }

    [HttpPost("reset-password"), AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var success = await _userService.ResetPasswordByTokenAsync(dto.Token, dto.NewPassword);
        return success
            ? Ok(new { message = "Password updated successfully." })
            : BadRequest(new { message = "Invalid or expired reset token." });
    }
}
