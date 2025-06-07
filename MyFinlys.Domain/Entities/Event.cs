using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public abstract class Event : Entity
{
    public EventType Type { get; protected set; }
    public EventPeriod Period { get; protected set; }
    public decimal Value { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public int InstallmentTotal { get; protected set; } = 0;
    public DateTime? DateInitial { get; protected set; }
    public DateTime? DateFinish { get; protected set; }
    public Affirmation AutoRealized { get; protected set; }
    public Guid AccountId { get; protected set; }
    public Account Account { get; protected set; } = null!;

    protected Event() { }

    protected Event(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        int installmentTotal,
        DateTime? dateInitial,
        DateTime? dateFinish,
        Affirmation autoRealized,
        Guid accountId
    )
    {
        Type = type;
        Period = period;
        Value = value;
        Description = description;
        InstallmentTotal = installmentTotal;
        DateInitial = dateInitial;
        DateFinish = dateFinish;
        AutoRealized = autoRealized;
        AccountId = accountId;
    }
}