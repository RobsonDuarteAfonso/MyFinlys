using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public class EventMonthly : Event
{
    public DateTime Due { get; private set; }

    private EventMonthly() { }

    public EventMonthly(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        Installment? installment,
        Affirmation autoRealized,
        Affirmation finished,
        Guid accountId,
        DateTime due
    ) : base(type, period, value, description, installment, autoRealized, finished, accountId)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId);
        Guard.AgainstInvalidDate(due, nameof(due));

        Due = due;
    }

    public void Update(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        Installment? installment,
        Affirmation autoRealized,
        Affirmation finished,
        Guid accountId,
        DateTime due)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId);
        Guard.AgainstInvalidDate(due, nameof(due));

        Type         = type;
        Period       = period;
        Value        = value;
        Description  = description;
        Installment  = installment;
        AutoRealized = autoRealized;
        Finished     = finished;
        AccountId    = accountId;
        Due          = due;
    }
}
