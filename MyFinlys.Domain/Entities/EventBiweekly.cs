using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class EventBiweekly : Event
{
    public DayOfWeek DayOfWeek { get; private set; }
    public DateTime StartDate { get; private set; }
    

    private EventBiweekly() { }

    public EventBiweekly(
        EventType type,
        decimal value,
        string description,
        DayOfWeek dayOfWeek,
        DateTime startDate,
        DateTime? dateInitial = null,
        DateTime? dateFinish = null,
        int installmentTotal = 0,
        Affirmation autoRealized = Affirmation.No,
        Guid? accountId = null
    ) : base(
        type,
        EventPeriod.Biweekly,
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
        StartDate = startDate;
    }
}