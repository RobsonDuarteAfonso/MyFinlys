using MyFinlys.Domain.Common;
using MyFinlys.Domain.ValueObjects;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public string? Avatar { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }
    public UserType Type { get; private set; }
    public string? Phone { get; private set; }
    public string PreferredLanguage { get; private set; } = "en";
    private readonly List<UserAccount> _userAccounts = [];
    public IReadOnlyCollection<UserAccount> UserAccounts => _userAccounts;

    //EF
    private User() { }

    private User(
        string name,
        Email email,
        Password password
    ) : base(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow)
    {
        Name = name;
        Email = email;
        Password = password;
        Type = UserType.User;
        PreferredLanguage = "en";
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

    public void AddAccount(Account account, AccessLevel accessLevel = AccessLevel.Owner)
    {
        _userAccounts.Add(UserAccount.Create(this.Id, account.Id, accessLevel));
    }

    public IEnumerable<Account> GetAccounts()
    {
        return _userAccounts.Select(ua => ua.Account);
    }

    public void SetPasswordResetToken(string token, DateTime expiry)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = expiry;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }

    public bool IsResetTokenValid(string token)
        => PasswordResetToken == token
           && PasswordResetTokenExpiry.HasValue
           && PasswordResetTokenExpiry.Value > DateTime.UtcNow;

    public void ResetPassword(string newPlainText)
    {
        var newVO = Password.Create(newPlainText);
        Password = newVO;
        ClearPasswordResetToken();
    }

    public void UpdateAvatar(string? avatar)
    {
        Avatar = avatar;
    }

    public void UpdateType(UserType type)
    {
        Guard.AgainstInvalidEnumValue(type, nameof(type));
        Type = type;
    }
    
    public void Update(string name, string email, string? avatar = null, string? phone = null, string? preferredLanguage = null)
    {
        Guard.AgainstLengthLessThan(name, 3, nameof(name));
        var emailVO = Email.Create(email);

        Name = name;
        Email = emailVO;
        if (avatar != null)
        {
            Avatar = avatar;
        }
        Phone = phone;
        if (!string.IsNullOrEmpty(preferredLanguage))
        {
            PreferredLanguage = preferredLanguage;
        }
    }
}
