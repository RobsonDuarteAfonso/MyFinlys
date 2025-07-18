namespace MyFinlys.Application.DTOs;

public class AccountDetailDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public List<UserBasicDto> Users { get; set; } = new();
}
