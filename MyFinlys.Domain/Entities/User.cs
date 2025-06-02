using MyFinlys.Domain.Entities;
using MyFinlys.Domain.ValueObjects;

public class User : Entity
{
    public string Name { get;  private set; } = string.Empty;
    public Email Email { get; private set; } = new Email("placeholder@email.com");
    public Password Password { get; private set; } = Password.FromHashed("fake-hash");

    //EF
    private User() { }

    public User(string name, string email, string password) : base()
    {

        if (name.Length < 3)
            throw new ArgumentException("Name must be at least 3 characters long.");

        Name = name;
        Email = new Email(email);
        Password = new Password(password);
    }
}
