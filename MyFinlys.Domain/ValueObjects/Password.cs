using MyFinlys.Domain.Validations;

namespace MyFinlys.Domain.ValueObjects;

public sealed class Password
{
    public string Value { get; private set; } = string.Empty;

    private Password() { }

    public Password(string value)
    {
        PasswordValidator.Validate(value);
        Value = BCrypt.Net.BCrypt.HashPassword(value);
    }

    public static Password FromHashed(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Invalid hash.");

        return new Password { Value = hash };
    }

    public bool Verify(string value) =>
        BCrypt.Net.BCrypt.Verify(value, Value);

    public override bool Equals(object? obj) =>
        obj is Password other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}