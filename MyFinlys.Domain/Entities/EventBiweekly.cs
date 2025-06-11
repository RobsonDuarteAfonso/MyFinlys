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
        Guid accountId,
        DayOfWeek dayOfWeek,
        DateTime startDate
    ) : base(type, period, value, description, installment, autoRealized, accountId)
    {
        DayOfWeek = dayOfWeek;
        StartDate = startDate;
    }
}