namespace MyFinlys.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Avatar { get; set; }
    public int Type { get; set; }
    public string? Phone { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public List<AccountSummaryDto> Accounts { get; set; } = [];
}
