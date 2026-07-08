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
        Category category,
        DayOfWeek dayOfWeek,
        DateTime? endDate = null
    ) : base(type, period, value, description, installment, autoRealized, finished, accountId, category, endDate)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId, category);
        Guard.AgainstInvalidEnumValue(dayOfWeek, nameof(dayOfWeek));
        DayOfWeek = dayOfWeek;
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
        Category category,
        DayOfWeek dayOfWeek,
        DateTime? endDate = null)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId, category);
        Guard.AgainstInvalidEnumValue(dayOfWeek, nameof(dayOfWeek));

        Type         = type;
        Period       = period;
        Value        = value;
        Description  = description;
        Installment  = installment;
        AutoRealized = autoRealized;
        Finished     = finished;
        AccountId    = accountId;
        Category     = category;
        DayOfWeek    = dayOfWeek;
        EndDate      = endDate;
    }
}
