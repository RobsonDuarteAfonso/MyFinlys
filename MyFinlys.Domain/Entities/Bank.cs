using MyFinlys.Domain.Common;

namespace MyFinlys.Domain.Entities;

public class Bank : Entity
{
    public string Name { get; private set; } = string.Empty;

    private Bank() { }

    private Bank(string name) : base()
    {
        Name = name;
    }

    public static Bank Create(string name)
    {
        Guard.AgainstLengthLessThan(name, 3, nameof(name));
        
        return new Bank(name);
    }
}