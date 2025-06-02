namespace MyFinlys.Domain.Entities;

public class Bank : Entity
{
    public string Name { get; private set; } = string.Empty;

    private Bank() { }

    public Bank(string name) : base()
    {
        if (name.Length < 3)
            throw new ArgumentException("Name must be at least 3 characters long.", nameof(name));

        Name = name;
    }
}