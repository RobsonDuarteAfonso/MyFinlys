using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Entities;

public class EventAnnual : Event
{
    public DateTime Due { get; private set; }

    private EventAnnual() { }

    public EventAnnual(
        EventType type,
        EventPeriod period,
        decimal value,
        string description,
        Installment? installment,
        Affirmation autoRealized,
        Affirmation finished,
        Guid accountId,
        Category category,
        DateTime due,
        DateTime? endDate = null,
        Guid? creditCardId = null
    ) : base(type, period, value, description, installment, autoRealized, finished, accountId, category, endDate, creditCardId)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId, category, creditCardId);
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
        Category category,
        DateTime due,
        DateTime? endDate = null,
        Guid? creditCardId = null)
    {
        ValidateEventBase(type, period, value, description, autoRealized, finished, accountId, category, creditCardId);
        Guard.AgainstInvalidDate(due, nameof(due));

        Type         = type;
        Period       = period;
        Value        = value;
        Description  = description;
        Installment  = installment;
        AutoRealized = autoRealized;
        Finished     = finished;
        AccountId    = accountId;
        Category     = category;
        Due          = due;
        EndDate      = endDate;
        CreditCardId = creditCardId;
    }
}
