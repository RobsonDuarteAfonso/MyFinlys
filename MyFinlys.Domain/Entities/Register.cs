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
    public Guid? EventId { get; private set; }
    public Event? Event { get; private set; }
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;
    public Category Category { get; private set; }
    public Guid? CreditCardId { get; private set; }
    public CreditCard? CreditCard { get; private set; }

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
        Guid? eventId,
        Guid accountId,
        Category category,
        Guid? creditCardId
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
        AccountId = accountId;
        Category = category;
        CreditCardId = creditCardId;
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
        Guid? eventId,
        Guid accountId,
        Category category,
        Guid? creditCardId = null
    )
    {
        Guard.AgainstInvalidDate(due, nameof(due));
        Guard.AgainstInvalidEnumValue(eventType, nameof(eventType));
        Guard.AgainstNegative(installmentCurrent, nameof(installmentCurrent));
        Guard.AgainstNegativeOrZero(value, nameof(value));
        Guard.AgainstNullOrEmpty(subdescription, nameof(subdescription));
        Guard.AgainstInvalidEnumValue(month, nameof(month));
        Guard.AgainstValueNotInRange(week, 1, 6, nameof(week));
        Guard.AgainstInvalidEnumValue(realized, nameof(realized));
        if (eventId.HasValue)
        {
            Guard.AgainstEmptyGuid(eventId.Value, nameof(eventId));
        }
        Guard.AgainstEmptyGuid(accountId, nameof(accountId));
        Guard.AgainstInvalidEnumValue(category, nameof(category));
        if (creditCardId.HasValue)
        {
            Guard.AgainstEmptyGuid(creditCardId.Value, nameof(creditCardId));
        }

        return new Register(due, eventType, installmentCurrent, value, subdescription, month, week, realized, eventId, accountId, category, creditCardId);
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
        Guid? eventId,
        Guid accountId,
        Category category,
        Guid? creditCardId = null)
    {
        Guard.AgainstInvalidDate(due, nameof(due));
        Guard.AgainstInvalidEnumValue(eventType, nameof(eventType));
        Guard.AgainstNegative(installmentCurrent, nameof(installmentCurrent));
        Guard.AgainstNegativeOrZero(value, nameof(value));
        Guard.AgainstNullOrEmpty(subdescription, nameof(subdescription));
        Guard.AgainstInvalidEnumValue(month, nameof(month));
        Guard.AgainstValueNotInRange(week, 1, 6, nameof(week));
        Guard.AgainstInvalidEnumValue(realized, nameof(realized));
        if (eventId.HasValue)
        {
            Guard.AgainstEmptyGuid(eventId.Value, nameof(eventId));
        }
        Guard.AgainstEmptyGuid(accountId, nameof(accountId));
        Guard.AgainstInvalidEnumValue(category, nameof(category));
        if (creditCardId.HasValue)
        {
            Guard.AgainstEmptyGuid(creditCardId.Value, nameof(creditCardId));
        }

        Due = due;
        EventType = eventType;
        InstallmentCurrent = installmentCurrent;
        Value = value;
        Subdescription = subdescription;
        Month = month;
        Week = week;
        Realized = realized;
        EventId = eventId;
        AccountId = accountId;
        Category = category;
        CreditCardId = creditCardId;
    }

    public void MarkRealized()
    {
        Realized = Affirmation.Yes;
    }

}