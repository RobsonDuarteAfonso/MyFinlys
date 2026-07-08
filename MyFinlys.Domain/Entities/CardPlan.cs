using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class CardPlan : Entity
{
    public Guid CardId { get; private set; }
    public CreditCard Card { get; private set; } = null!;
    public Guid? AccountId { get; private set; }
    public Account? Account { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Category Category { get; private set; }

    /// <summary>Valor original da compra SEM juros.</summary>
    public decimal OriginalAmount { get; private set; }

    /// <summary>Valor da parcela mensal COM juros do parcelamento.</summary>
    public decimal MonthlyAmount { get; private set; }

    public int TotalInstallments { get; private set; }

    /// <summary>Número da parcela corrente (1-based). Começa em 1 mesmo que o plano já esteja em andamento.</summary>
    public int CurrentInstallment { get; private set; }

    /// <summary>Saldo devedor remanescente (pode ser informado manualmente para planos já em andamento).</summary>
    public decimal RemainingBalance { get; private set; }

    /// <summary>Dia do mês em que a parcela vence (ex: 15).</summary>
    public int DueDay { get; private set; }

    /// <summary>Mês da primeira parcela cadastrada (pode ser retroativo para planos já em andamento).</summary>
    public Month StartMonth { get; private set; }
    public int StartYear { get; private set; }

    public bool IsActive => CurrentInstallment <= TotalInstallments && RemainingBalance > 0;

    private CardPlan() { }

    private CardPlan(
        Guid cardId,
        Guid? accountId,
        string description,
        Category category,
        decimal originalAmount,
        decimal monthlyAmount,
        int totalInstallments,
        int currentInstallment,
        decimal remainingBalance,
        int dueDay,
        Month startMonth,
        int startYear
    ) : base()
    {
        CardId = cardId;
        AccountId = accountId;
        Description = description;
        Category = category;
        OriginalAmount = originalAmount;
        MonthlyAmount = monthlyAmount;
        TotalInstallments = totalInstallments;
        CurrentInstallment = currentInstallment;
        RemainingBalance = remainingBalance;
        DueDay = dueDay;
        StartMonth = startMonth;
        StartYear = startYear;
    }

    public static CardPlan Create(
        Guid cardId,
        Guid? accountId,
        string description,
        Category category,
        decimal originalAmount,
        decimal monthlyAmount,
        int totalInstallments,
        int currentInstallment,
        decimal remainingBalance,
        int dueDay,
        Month startMonth,
        int startYear
    )
    {
        Guard.AgainstEmptyGuid(cardId, nameof(cardId));
        if (accountId.HasValue) Guard.AgainstEmptyGuid(accountId.Value, nameof(accountId));
        Guard.AgainstNullOrEmpty(description, nameof(description));
        Guard.AgainstInvalidEnumValue(category, nameof(category));
        Guard.AgainstNegativeOrZero(originalAmount, nameof(originalAmount));
        Guard.AgainstNegativeOrZero(monthlyAmount, nameof(monthlyAmount));
        Guard.AgainstValueNotInRange(totalInstallments, 1, 600, nameof(totalInstallments));
        Guard.AgainstValueNotInRange(currentInstallment, 1, totalInstallments, nameof(currentInstallment));
        Guard.AgainstNegativeOrZero(remainingBalance, nameof(remainingBalance));
        Guard.AgainstValueNotInRange(dueDay, 1, 31, nameof(dueDay));
        Guard.AgainstInvalidEnumValue(startMonth, nameof(startMonth));

        return new CardPlan(
            cardId, accountId, description, category,
            originalAmount, monthlyAmount, totalInstallments,
            currentInstallment, remainingBalance, dueDay,
            startMonth, startYear
        );
    }

    /// <summary>Avança a parcela corrente e recalcula o saldo após pagamento.</summary>
    public void AdvanceInstallment()
    {
        if (CurrentInstallment < TotalInstallments)
        {
            CurrentInstallment++;
        }
        RemainingBalance = Math.Max(0, RemainingBalance - MonthlyAmount);
        SetUpdatedAt();
    }

    /// <summary>Atualiza o saldo manualmente (ex: após ajuste).</summary>
    public void UpdateRemainingBalance(decimal balance)
    {
        Guard.AgainstNegativeOrZero(balance, nameof(balance));
        RemainingBalance = balance;
        SetUpdatedAt();
    }
}
