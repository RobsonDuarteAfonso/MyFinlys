namespace MyFinlys.Application.DTOs;

public class AccountCreateDto
{
    public string Number { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid BankId { get; set; }
    public IEnumerable<Guid> UserIds { get; set; } = [];
}
