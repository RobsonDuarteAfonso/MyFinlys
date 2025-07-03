namespace MyFinlys.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<AccountSummaryDto> Accounts { get; set; } = new();
}
