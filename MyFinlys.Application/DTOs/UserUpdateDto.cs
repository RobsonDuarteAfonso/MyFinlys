namespace MyFinlys.Application.DTOs;

public class UserUpdateDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public string? PreferredLanguage { get; set; }
}
