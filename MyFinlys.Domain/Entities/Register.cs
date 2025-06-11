using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

public class Register : Entity
{
    public DateTime Due { get; private set; }
    EventType EventType { get; private set; }
    decimal Value { get; private set; }
    string Subdescription { get; private set; }
    Month Month { get; private set; }
    DayOfWeek DayOfWeek { get; private set; }
    Affirmation Realized { get; private set; }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    
    public Register(Parameters)
    {
        
    }
}