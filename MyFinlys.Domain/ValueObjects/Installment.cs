using MyFinlys.Shared.Validations;

namespace MyFinlys.Domain.ValueObjects
{
    public sealed class Installment : ValueObjectBase<Installment>
    {
        public int InstallmentTotal { get; }
        public int InstallmentCurrent { get; }
        public decimal InstallmentValue { get; }
        public DateTime? DateInitial { get; }
        public DateTime? DateFinish { get; }

        private Installment() { }

        public Installment(int installmentTotal, int installmentCurrent, decimal installmentValue, DateTime? dateInitial, DateTime? dateFinish)
        {
            InstallmentValidator.Validate(installmentTotal, installmentCurrent, installmentValue, dateInitial, dateFinish);

            InstallmentTotal = installmentTotal;
            InstallmentCurrent = installmentCurrent;
            InstallmentValue = installmentValue;
            DateInitial = dateInitial;
            DateFinish = dateFinish;
        }

        // With Methods
        public Installment WithInstallmentCurrent(int newCurrent)
            => new(InstallmentTotal, newCurrent, InstallmentValue, DateInitial, DateFinish);

    }
}
