using MyFinlys.Shared.Validations;

namespace MyFinlys.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; private set; } = string.Empty;

    public Email(string value)
    {
        EmailValidator.Validate(value);
        Value = value;
    }

    // EF Core
    private Email() { }

    public override string ToString() => Value;

    public override bool Equals(object? obj) =>
        obj is Email other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Email email) => email.Value;
}