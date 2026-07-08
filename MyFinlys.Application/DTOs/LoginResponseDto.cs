namespace MyFinlys.Application.DTOs;
public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public int Type { get; set; }
    public string? Phone { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public DateTime ExpiresAt { get; set; }
}