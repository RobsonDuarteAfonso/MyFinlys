using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public class Balance : Entity
{
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;
    public Year Year { get; private set; } = null!;
    public Month Month { get; private set; }
    public decimal Amount { get; private set; }

    private Balance() { }

    private Balance(Guid accountId, Year year, Month month, decimal amount)
    {
        Guard.AgainstInvalidEnumValue(month, nameof(month));

        AccountId = accountId;
        Year = year;
        Month = month;
        Amount = amount;
    }

    public static Balance Create(Guid accountId, int year, Month month, decimal amount)
    {
        var yearVO = Year.Create(year);
        return new Balance(accountId, yearVO, month, amount);
    }

    public void UpdateAmount(decimal amount)
    {
        Amount = amount;
    }
}
