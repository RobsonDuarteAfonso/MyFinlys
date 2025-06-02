using System.Text.RegularExpressions;

namespace MyFinlys.Domain.Validations;

public static partial class PasswordValidator
{
    [GeneratedRegex("[A-Z]")]
    private static partial Regex UppercaseRegex();

    [GeneratedRegex("[a-z]")]
    private static partial Regex LowercaseRegex();

    [GeneratedRegex("[0-9]")]
    private static partial Regex DigitRegex();

    [GeneratedRegex(@"[\W_]")]
    private static partial Regex SymbolRegex();

    public static void Validate(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty.");

        if (plainPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters long.");

        if (!UppercaseRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one uppercase letter.");

        if (!LowercaseRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one lowercase letter.");

        if (!DigitRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one digit.");

        if (!SymbolRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one symbol.");
    }
}
