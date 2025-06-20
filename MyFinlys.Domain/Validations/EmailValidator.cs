using System.Text.RegularExpressions;
using MyFinlys.Domain.Common;

namespace MyFinlys.Domain.Validations;

public static partial class EmailValidator
{
    private static readonly Regex EmailRegex = MyRegex();

    public static void Validate(string plainEmail)
    {
        Guard.AgainstNullOrEmpty(plainEmail, nameof(plainEmail));

        if (!EmailRegex.IsMatch(plainEmail))
            throw new ArgumentException("Invalid email format.", nameof(plainEmail));
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
