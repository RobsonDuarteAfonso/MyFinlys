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

    public Account(string number, AccountType type, Guid userId, Guid bankId) : base()
    {
        if (number.Length < 3)
            throw new ArgumentException("Account Number must be at least 3 characters long.", nameof(number));

        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid account type!", nameof(type));

        if (userId == Guid.Empty)
            throw new ArgumentException("User cannot be empty.", nameof(userId));

        if (bankId == Guid.Empty)
            throw new ArgumentException("Bank cannot be empty.", nameof(bankId));

        Number = number;
        Type = type;
        UserId = userId;
        BankId = bankId;
    }
}