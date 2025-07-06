using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class Account : Entity
{
    public string Number { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public Guid BankId { get; private set; }
    public Bank Bank { get; private set; } = null!;
    private readonly List<UserAccount> _userAccounts = [];
    public IReadOnlyCollection<UserAccount> UserAccounts => _userAccounts;
    private readonly List<Balance> _balances = [];
    public IReadOnlyCollection<Balance> Balances => _balances;

    private Account() { }

    private Account(string number, AccountType type, Guid bankId) : base()
    {
        Number = number;
        Type = type;
        BankId = bankId;
    }

    public static Account Create(string number, AccountType type, Guid bankId)
    {
        Guard.AgainstLengthLessThan(number, 3, nameof(number));
        Guard.AgainstInvalidEnumValue(type, nameof(type));
        Guard.AgainstEmptyGuid(bankId, nameof(bankId));

        return new Account(number, type, bankId);
    }

    public void Update(string number, AccountType type, Guid bankId)
    {
        Guard.AgainstLengthLessThan(number, 3, nameof(number));
        Guard.AgainstInvalidEnumValue(type, nameof(type));
        Guard.AgainstEmptyGuid(bankId, nameof(bankId));

        Number = number;
        Type = type;
        BankId = bankId;
    }

    
    public void AddUser(User user)
    {
        _userAccounts.Add(UserAccount.Create(user.Id, this.Id));
    }

    public void AddBalance(Balance balance)
    {
        _balances.Add(balance);
    }

    public IEnumerable<User> GetUsers()
    {
        return _userAccounts.Select(ua => ua.User);
    }    
}