using MyFinlys.Domain.Common;

namespace MyFinlys.Shared.Validations;

public static partial class InstallmentValidator
{
    public static void Validate(int installmentTotal, int installmentCurrent, decimal installmentValue, DateTime? dateInitial, DateTime? dateFinish)
    {
        Guard.AgainstNegativeOrZero(installmentTotal, nameof(installmentTotal));
        Guard.AgainstValueNotInRange(installmentCurrent, 1, installmentTotal, nameof(installmentCurrent));
        Guard.AgainstNegative(installmentValue, nameof(installmentValue));
        Guard.AgainstDateEarlierThan(dateInitial, dateFinish, "Finish date cannot be before initial date.");
    }
}
