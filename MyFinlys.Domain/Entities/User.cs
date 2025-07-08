using MyFinlys.Domain.Common;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    private readonly List<UserAccount> _userAccounts = [];
    public IReadOnlyCollection<UserAccount> UserAccounts => _userAccounts;

    //EF
    private User() { }

    private User(
        string name,
        Email email,
        Password password
    ) : base()
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public static User Create(string name, string email, string password)
    {
        Guard.AgainstLengthLessThan(name, 3, nameof(name));

        var emailVO = Email.Create(email);
        var passwordVO = Password.Create(password);

        return new User(
            name,
            emailVO,
            passwordVO
        );
    }

    public bool VerifyPassword(string plainText)
        => Password.Verify(plainText);

    public void ChangePassword(string newPlainText)
    {
        var newVO = Password.Create(newPlainText);
        Password = newVO;
    }    

    public void AddAccount(Account account)
    {
        _userAccounts.Add(UserAccount.Create(this.Id, account.Id));
    }

    public IEnumerable<Account> GetAccounts()
    {
        return _userAccounts.Select(ua => ua.Account);
    }
    
    public void Update(string name, string email)
{
    Guard.AgainstLengthLessThan(name, 3, nameof(name));
    var emailVO = Email.Create(email);

    Name = name;
    Email = Email.Create(emailVO);
}

}
