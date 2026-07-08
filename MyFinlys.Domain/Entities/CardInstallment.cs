using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class CardInstallment : Entity
{
    public Guid CardPurchaseId { get; private set; }
    public CardPurchase CardPurchase { get; private set; } = null!;
    public int InstallmentNumber { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; } // Billing month representation (e.g. 1st day of the billing month)
    public Affirmation Realized { get; private set; } // Yes if paid, No if unpaid

    private CardInstallment() { }

    private CardInstallment(
        Guid cardPurchaseId,
        int installmentNumber,
        decimal amount,
        DateTime dueDate,
        Affirmation realized
    ) : base()
    {
        CardPurchaseId = cardPurchaseId;
        InstallmentNumber = installmentNumber;
        Amount = amount;
        DueDate = dueDate;
        Realized = realized;
    }

    public static CardInstallment Create(
        Guid cardPurchaseId,
        int installmentNumber,
        decimal amount,
        DateTime dueDate,
        Affirmation realized = Affirmation.No
    )
    {
        Guard.AgainstEmptyGuid(cardPurchaseId, nameof(cardPurchaseId));
        Guard.AgainstValueNotInRange(installmentNumber, 1, 120, nameof(installmentNumber));
        Guard.AgainstNegativeOrZero(amount, nameof(amount));
        Guard.AgainstInvalidDate(dueDate, nameof(dueDate));
        Guard.AgainstInvalidEnumValue(realized, nameof(realized));

        return new CardInstallment(cardPurchaseId, installmentNumber, amount, dueDate, realized);
    }

    public void MarkAsRealized(Affirmation realized)
    {
        Guard.AgainstInvalidEnumValue(realized, nameof(realized));
        Realized = realized;
    }

    public void UpdateDueDate(DateTime dueDate)
    {
        Guard.AgainstInvalidDate(dueDate, nameof(dueDate));
        DueDate = dueDate;
    }
}
