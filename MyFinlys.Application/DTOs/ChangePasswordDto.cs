namespace MyFinlys.Application.DTOs;
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword     { get; set; } = null!;
}