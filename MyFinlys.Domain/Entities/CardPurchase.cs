using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class CardPurchase : Entity
{
    public string Description { get; private set; } = string.Empty;
    public DateTime PurchaseDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public int TotalInstallments { get; private set; }
    public Guid CardId { get; private set; }
    public CreditCard Card { get; private set; } = null!;
    public Guid? AccountId { get; private set; }
    public Account? Account { get; private set; }
    public Category Category { get; private set; }
    public PurchaseType Type { get; private set; }

    private readonly List<CardInstallment> _installments = [];
    public IReadOnlyCollection<CardInstallment> Installments => _installments;

    private CardPurchase() { }

    private CardPurchase(
        string description,
        DateTime purchaseDate,
        decimal totalAmount,
        int totalInstallments,
        Guid cardId,
        Guid? accountId,
        Category category,
        PurchaseType type
    ) : base()
    {
        Description = description;
        PurchaseDate = purchaseDate;
        TotalAmount = totalAmount;
        TotalInstallments = totalInstallments;
        CardId = cardId;
        AccountId = accountId;
        Category = category;
        Type = type;
    }

    public static CardPurchase Create(
        string description,
        DateTime purchaseDate,
        decimal totalAmount,
        int totalInstallments,
        Guid cardId,
        Guid? accountId,
        Category category,
        PurchaseType type = PurchaseType.Credit
    )
    {
        Guard.AgainstNullOrEmpty(description, nameof(description));
        Guard.AgainstInvalidDate(purchaseDate, nameof(purchaseDate));
        Guard.AgainstNegativeOrZero(totalAmount, nameof(totalAmount));
        Guard.AgainstValueNotInRange(totalInstallments, 1, 120, nameof(totalInstallments));
        Guard.AgainstEmptyGuid(cardId, nameof(cardId));
        if (accountId.HasValue)
        {
            Guard.AgainstEmptyGuid(accountId.Value, nameof(accountId));
        }
        Guard.AgainstInvalidEnumValue(category, nameof(category));
        Guard.AgainstInvalidEnumValue(type, nameof(type));

        return new CardPurchase(description, purchaseDate, totalAmount, totalInstallments, cardId, accountId, category, type);
    }

    public void AddInstallment(CardInstallment installment)
    {
        _installments.Add(installment);
    }
}
