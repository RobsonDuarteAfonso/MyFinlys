using MyFinlys.Shared.Validations;

namespace MyFinlys.Domain.ValueObjects
{
    public sealed class Installment : ValueObjectBase<Installment>
    {
        public int InstallmentTotal { get; private set; }
        public int InstallmentCurrent { get; private set; }
        public decimal InstallmentValue { get; private set; }
        public DateTime? DateInitial { get; private set; }
        public DateTime? DateFinish { get; private set; }

        private Installment() { } // Para EF Core

        private Installment(int installmentTotal, int installmentCurrent, decimal installmentValue, DateTime? dateInitial, DateTime? dateFinish)
        {
            InstallmentTotal = installmentTotal;
            InstallmentCurrent = installmentCurrent;
            InstallmentValue = installmentValue;
            DateInitial = dateInitial;
            DateFinish = dateFinish;
        }

        // Factory de criação com validação
        public static Installment Create(int installmentTotal, int installmentCurrent, decimal installmentValue, DateTime? dateInitial, DateTime? dateFinish)
        {
            InstallmentValidator.Validate(installmentTotal, installmentCurrent, installmentValue, dateInitial, dateFinish);

            return new Installment(
                installmentTotal,
                installmentCurrent,
                installmentValue,
                dateInitial,
                dateFinish
            );
        }

        // With Methods (imutabilidade controlada)
        public Installment WithInstallmentCurrent(int newCurrent)
            => new(
                InstallmentTotal,
                newCurrent,
                InstallmentValue,
                DateInitial,
                DateFinish
            );
    }
}
