using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public abstract class Event : Entity
{
    public EventType Type { get; protected set; }
    public EventPeriod Period { get; protected set; }
    public decimal Value { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public Installment? Installment { get; protected set; }
    public Affirmation AutoRealized { get; protected set; }
    public Affirmation Finished { get; protected set; }
    public Guid AccountId { get; protected set; }
    public Account Account { get; protected set; } = null!;

    protected Event() { }

    protected Event(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        Installment? installment,
        Affirmation autoRealized,
        Affirmation finished,
        Guid accountId
    )
    {
        Type = type;
        Period = period;
        Value = value;
        Description = description;
        Installment = installment;
        AutoRealized = autoRealized;
        Finished = finished;
        AccountId = accountId;
    }
    
    protected static void ValidateEventBase(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        Affirmation autoRealized,
        Affirmation finished,
        Guid accountId
    )
    {
        Guard.AgainstInvalidEnumValue(type, nameof(type));
        Guard.AgainstInvalidEnumValue(period, nameof(period));
        Guard.AgainstNegativeOrZero(value, nameof(value));
        Guard.AgainstNullOrEmpty(description, nameof(description));
        Guard.AgainstInvalidEnumValue(autoRealized, nameof(autoRealized));
        Guard.AgainstInvalidEnumValue(finished, nameof(finished));
        Guard.AgainstEmptyGuid(accountId, nameof(accountId));
    }
}