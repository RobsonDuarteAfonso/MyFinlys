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
            Guid accountId,
            DayOfWeek dayOfWeek
        ) : base(type, period, value, description, installment, autoRealized, accountId)
        {
            DayOfWeek = dayOfWeek;
        }

}