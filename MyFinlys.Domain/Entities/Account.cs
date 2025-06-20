using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class Account : Entity
{
    public string Number { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid BankId { get; private set; }
    public Bank Bank { get; private set; } = null!;

    private Account() { }

    private Account(string number, AccountType type, Guid userId, Guid bankId) : base()
    {
        Number = number;
        Type = type;
        UserId = userId;
        BankId = bankId;
    }

    public static Account Create(string number, AccountType type, Guid userId, Guid bankId)
    {
        Guard.AgainstLengthLessThan(number, 3, nameof(number));
        Guard.AgainstInvalidEnumValue(type, nameof(type));
        Guard.AgainstEmptyGuid(userId, nameof(userId));
        Guard.AgainstEmptyGuid(bankId, nameof(bankId));

        return new Account(number, type, userId, bankId);
    }
}