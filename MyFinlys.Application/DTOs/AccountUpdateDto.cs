namespace MyFinlys.Application.DTOs;

public class AccountUpdateDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid BankId { get; set; }
    public IEnumerable<Guid> UserIds { get; set; } = [];
}
