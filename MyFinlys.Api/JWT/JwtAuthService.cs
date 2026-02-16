using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFinlys.Api.JWT;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;

namespace MyFinlys.Api.Services;

public class JwtAuthService
{
    private readonly IUserService _userService;
    private readonly JwtSettings  _jwt;

    public JwtAuthService(IUserService userService, IOptions<JwtSettings> opts)
    {
        _userService = userService;
        _jwt         = opts.Value;
    }

    public async Task<LoginResponseDto?> GenerateTokenAsync(LoginRequestDto dto)
    {
        var valid = await _userService.ValidateCredentialsAsync(dto.Email, dto.Password);
        if (!valid) return null;

        var user = await _userService.GetByEmailAsync(dto.Email);
        if (user is null) return null;

        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwt.DurationMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, dto.Email),
            new Claim("UserId", user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer:             _jwt.Issuer,
            audience:           _jwt.Audience,
            claims:             claims,
            expires:            expires,
            signingCredentials: creds
        );

        return new LoginResponseDto
        {
            Token     = new JwtSecurityTokenHandler().WriteToken(token),
            Name      = user.Name,
            ExpiresAt = expires
        };
    }

    public async Task<bool> ChangePasswordAsync(string email, ChangePasswordDto dto)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user is null) return false;

        return await _userService.ChangePasswordAsync(user.Id, dto.CurrentPassword, dto.NewPassword);
    }
}
