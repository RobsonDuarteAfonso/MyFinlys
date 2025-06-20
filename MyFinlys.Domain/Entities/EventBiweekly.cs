using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public class EventBiweekly : Event
{
    public DayOfWeek DayOfWeek { get; private set; }
    public DateTime StartDate { get; private set; }
    

    private EventBiweekly() { }

    public EventBiweekly(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        Installment? installment,
        Affirmation autoRealized,
        Affirmation finished,
        Guid accountId,
        DayOfWeek dayOfWeek,
        DateTime startDate
    ) : base(type, period, value, description, installment, autoRealized, finished, accountId)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId);
        Guard.AgainstInvalidEnumValue(dayOfWeek, nameof(dayOfWeek));
        Guard.AgainstInvalidDate(startDate, nameof(startDate));
                
        DayOfWeek = dayOfWeek;
        StartDate = startDate;
    }
}