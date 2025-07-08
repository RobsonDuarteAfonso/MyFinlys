namespace MyFinlys.Application.DTOs;

public class BalanceCreateDto
{
    public Guid AccountId { get; set; }
    public int Year { get; set; }
    public string Month { get; set; } = null!;
    public decimal Amount { get; set; }
}
