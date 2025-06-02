using System.Text.RegularExpressions;

namespace MyFinlys.Shared.Validations;

public static partial class EmailValidator
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    public static void Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        if (!EmailRegex().IsMatch(email))
            throw new ArgumentException("Invalid email format.");
    }
}