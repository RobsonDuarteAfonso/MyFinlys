namespace MyFinlys.Domain.Common;

public static class Guard
{
    public static void AgainstNullOrEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or empty.", paramName);
    }

    public static void AgainstLengthLessThan(string value, int minLength, string paramName)
    {
        if (value.Length < minLength)
            throw new ArgumentException($"Value must be at least {minLength} characters long.", paramName);
    }

    public static void AgainstNegativeOrZero(decimal value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", paramName);
    }

    public static void AgainstEmptyGuid(Guid value, string paramName)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("GUID cannot be empty.", paramName);
    }

    public static void AgainstInvalidDate(DateTime value, string paramName)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Invalid date.", paramName);
    }

    public static void AgainstNegative(decimal value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
    }

    public static void AgainstValueGreaterThan(int value, int max, string paramName)
    {
        if (value > max)
            throw new ArgumentOutOfRangeException(paramName, $"Value must be less than or equal to {max}.");
    }

    public static void AgainstValueNotInRange(int value, int min, int max, string paramName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
    }

    public static void AgainstDateEarlierThan(DateTime? first, DateTime? second, string message)
    {
        if (first.HasValue && second.HasValue && second < first)
            throw new ArgumentException(message);
    }

    public static void AgainstIntNotInRange(int value, int min, int max, string paramName, string? message = null)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"Value must be between {min} and {max}.");

    }
    public static void AgainstYearOutsideAllowedRange(int year, string paramName, int minYear = 1900, int maxYearsInFuture = 50)
    {
        int currentYear = DateTime.UtcNow.Year;
        int maxYear = currentYear + maxYearsInFuture;

        if (year < minYear || year > maxYear)
            throw new ArgumentOutOfRangeException(paramName, $"Invalid year. Year must be between {minYear} and {maxYear}.");
    }

    public static void AgainstInvalidEnumValue<TEnum>(TEnum value, string paramName) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentException($"Invalid value for enum type '{typeof(TEnum).Name}'.", paramName);
    }
}
