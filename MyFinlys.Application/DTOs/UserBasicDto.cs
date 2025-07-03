namespace MyFinlys.Application.DTOs;

public class UserBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}
