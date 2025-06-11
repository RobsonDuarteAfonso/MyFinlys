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
        Guid accountId,
        DateTime due
    ) : base(type, period, value, description, installment, autoRealized, accountId)
    {
        Due = due;
    }

}