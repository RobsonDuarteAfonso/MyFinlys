using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public class EventWeekly : Event
{
    public DayOfWeek DayOfWeek { get; private set; }

    private EventWeekly() { }

    public EventWeekly(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        Installment? installment,
        Affirmation autoRealized,
        Affirmation finished,
        Guid accountId,
        DayOfWeek dayOfWeek
    ) : base(type, period, value, description, installment, autoRealized, finished, accountId)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId);
        Guard.AgainstInvalidEnumValue(dayOfWeek, nameof(dayOfWeek));
        
        DayOfWeek = dayOfWeek;
    }

}