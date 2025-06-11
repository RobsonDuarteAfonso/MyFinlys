namespace MyFinlys.Shared.Validations;

public static partial class InstallmentValidator
{
    public static void Validate(int installmentTotal, int installmentCurrent, decimal installmentValue, DateTime? dateInitial, DateTime? dateFinish)
    {
        if (installmentTotal > 0)
            throw new ArgumentOutOfRangeException(nameof(installmentTotal), "Installment total must be greater than 0.");

        if (installmentCurrent > 0 && installmentCurrent < installmentTotal)
            throw new ArgumentOutOfRangeException(nameof(installmentCurrent), $"Installment current must be between 1 and {installmentTotal}.");

        if (installmentValue < 0)
            throw new ArgumentOutOfRangeException(nameof(installmentValue), "Installment value cannot be negative.");

        if (dateInitial.HasValue && dateFinish < dateInitial)
            throw new ArgumentException("Finish date cannot be before initial date.");

        if (dateFinish.HasValue && dateFinish > dateInitial)
            throw new ArgumentException("Finish date cannot be after finish date.");   
    }
}