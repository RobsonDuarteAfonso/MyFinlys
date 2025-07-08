using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class Register : Entity
{
    public DateTime Due { get; private set; }
    public EventType EventType { get; private set; }
    public int InstallmentCurrent { get; private set; }
    public decimal Value { get; private set; }
    public string Subdescription { get; private set; } = string.Empty;
    public Month Month { get; private set; }
    public int Week { get; private set; }
    public Affirmation Realized { get; private set; }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;

    private Register() { }

    private Register(
        DateTime due,
        EventType eventType,
        int installmentCurrent,
        decimal value,
        string subdescription,
        Month month,
        int week,
        Affirmation realized,
        Guid eventId
    ) : base()
    {
        Due = due;
        EventType = eventType;
        InstallmentCurrent = installmentCurrent;
        Value = value;
        Subdescription = subdescription;
        Month = month;
        Week = week;
        Realized = realized;
        EventId = eventId;
    }

    public static Register Create(
        DateTime due,
        EventType eventType,
        int installmentCurrent,
        decimal value,
        string subdescription,
        Month month,
        int week,
        Affirmation realized,
        Guid eventId
    )
    {
        Guard.AgainstInvalidDate(due, nameof(due));
        Guard.AgainstInvalidEnumValue(eventType, nameof(eventType));
        Guard.AgainstNegativeOrZero(installmentCurrent, nameof(installmentCurrent));
        Guard.AgainstNegativeOrZero(value, nameof(value));
        Guard.AgainstNullOrEmpty(subdescription, nameof(subdescription));
        Guard.AgainstInvalidEnumValue(month, nameof(month));
        Guard.AgainstValueNotInRange(week, 1, 5, nameof(week));
        Guard.AgainstInvalidEnumValue(realized, nameof(realized));
        Guard.AgainstEmptyGuid(eventId, nameof(eventId));

        return new Register(due, eventType, installmentCurrent, value, subdescription, month, week, realized, eventId);
    }
    
    public void Update(
        DateTime due,
        EventType eventType,
        int installmentCurrent,
        decimal value,
        string subdescription,
        Month month,
        int week,
        Affirmation realized,
        Guid eventId)
    {
        Guard.AgainstInvalidDate(due, nameof(due));
        Guard.AgainstInvalidEnumValue(eventType, nameof(eventType));
        Guard.AgainstNegativeOrZero(installmentCurrent, nameof(installmentCurrent));
        Guard.AgainstNegativeOrZero(value, nameof(value));
        Guard.AgainstNullOrEmpty(subdescription, nameof(subdescription));
        Guard.AgainstInvalidEnumValue(month, nameof(month));
        Guard.AgainstValueNotInRange(week, 1, 5, nameof(week));
        Guard.AgainstInvalidEnumValue(realized, nameof(realized));
        Guard.AgainstEmptyGuid(eventId, nameof(eventId));

        Due = due;
        EventType = eventType;
        InstallmentCurrent = installmentCurrent;
        Value = value;
        Subdescription = subdescription;
        Month = month;
        Week = week;
        Realized = realized;
        EventId = eventId;
    }

}