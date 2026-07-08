using MyFinlys.Domain.Common;

namespace MyFinlys.Domain.Entities;

public class CreditCard : Entity
{
    public string Name { get; private set; } = string.Empty;
    public decimal Limit { get; private set; }
    public int ClosingDay { get; private set; }
    public int DueDay { get; private set; }
    public Guid? AccountId { get; private set; }
    public Account? Account { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private CreditCard() { }

    private CreditCard(
        string name,
        decimal limit,
        int closingDay,
        int dueDay,
        Guid? accountId,
        Guid userId
    ) : base()
    {
        Name = name;
        Limit = limit;
        ClosingDay = closingDay;
        DueDay = dueDay;
        AccountId = accountId;
        UserId = userId;
    }

    public static CreditCard Create(
        string name,
        decimal limit,
        int closingDay,
        int dueDay,
        Guid? accountId,
        Guid userId
    )
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNegativeOrZero(limit, nameof(limit));
        Guard.AgainstValueNotInRange(closingDay, 1, 31, nameof(closingDay));
        Guard.AgainstValueNotInRange(dueDay, 1, 31, nameof(dueDay));
        Guard.AgainstEmptyGuid(userId, nameof(userId));

        return new CreditCard(name, limit, closingDay, dueDay, accountId, userId);
    }

    public void Update(
        string name,
        decimal limit,
        int closingDay,
        int dueDay,
        Guid? accountId,
        Guid userId
    )
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNegativeOrZero(limit, nameof(limit));
        Guard.AgainstValueNotInRange(closingDay, 1, 31, nameof(closingDay));
        Guard.AgainstValueNotInRange(dueDay, 1, 31, nameof(dueDay));
        Guard.AgainstEmptyGuid(userId, nameof(userId));

        Name = name;
        Limit = limit;
        ClosingDay = closingDay;
        DueDay = dueDay;
        AccountId = accountId;
        UserId = userId;
    }
}
