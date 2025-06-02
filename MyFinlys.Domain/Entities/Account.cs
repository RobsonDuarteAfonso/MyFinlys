using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class Account : Entity
{
    public string Number { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Account() { }

    public Account(string number, AccountType type, Guid userId) : base()
    {
        if (number.Length < 3)
            throw new ArgumentException("Account Number must be at least 3 characters long.", nameof(number));
        
        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid account type!", nameof(type));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        Number = number;
        Type = type;
        UserId = userId;
    }
}