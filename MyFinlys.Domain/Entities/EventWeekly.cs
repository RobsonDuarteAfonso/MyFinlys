using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class EventWeekly : Event
{
    public DayOfWeek DayOfWeek { get; private set; }

    private EventWeekly() { }

    public EventWeekly(
        EventType type,
        decimal value,
        string description,
        DayOfWeek dayOfWeek,
        DateTime? dateInitial = null,
        DateTime? dateFinish = null,
        int installmentTotal = 0,
        Affirmation autoRealized = Affirmation.No,
        Guid? accountId = null
    ) : base(
        type,
        EventPeriod.Weekly,
        value,
        description,
        installmentTotal,
        dateInitial,
        dateFinish,
        autoRealized,
        accountId ?? Guid.Empty
    )
    {
        DayOfWeek = dayOfWeek;
    }

}