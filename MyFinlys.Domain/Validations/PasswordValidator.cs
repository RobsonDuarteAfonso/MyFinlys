using System.Text.RegularExpressions;
using MyFinlys.Domain.Common;

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
        Guard.AgainstNullOrEmpty(plainPassword, nameof(plainPassword));
        Guard.AgainstLengthLessThan(plainPassword, 6, nameof(plainPassword));

        if (!UppercaseRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(plainPassword));

        if (!LowercaseRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one lowercase letter.", nameof(plainPassword));

        if (!DigitRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one digit.", nameof(plainPassword));

        if (!SymbolRegex().IsMatch(plainPassword))
            throw new ArgumentException("Password must contain at least one symbol.", nameof(plainPassword));
    }
}
