using MyFinlys.Domain.Common;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public ICollection<Account> Accounts { get; private set; } = [];

    //EF
    private User() { }

    private User(Guid id, DateTime createdAt, DateTime updatedAt,
                 string name, Email email, Password password)
        : base(id, createdAt, updatedAt)
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
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow,
            name,
            emailVO,
            passwordVO
        );
    }    
}
