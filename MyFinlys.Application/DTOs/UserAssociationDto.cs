namespace MyFinlys.Application.DTOs;

public class UserAssociationDto
{
    public Guid UserId { get; set; }
    public string AccessLevel { get; set; } = string.Empty;
}
