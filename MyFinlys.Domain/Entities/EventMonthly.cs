using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class EventMonthly : Event
{
    public DateTime Due { get; private set; }

    private EventMonthly() { }


    public EventMonthly(
        EventType type,
        decimal value,
        string description,
        DateTime due,
        DateTime? dateInitial = null,
        DateTime? dateFinish = null,
        int installmentTotal = 0,
        Affirmation autoRealized = Affirmation.No,
        Guid? accountId = null
    ) : base(
        type,
        EventPeriod.Monthly,
        value,
        description,
        installmentTotal,
        dateInitial,
        dateFinish,
        autoRealized,
        accountId ?? Guid.Empty
    )
    {
        Due = due;
    }

}