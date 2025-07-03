namespace MyFinlys.Application.DTOs;

public class EventBiweeklyDto : EventBaseDto
{
    public string DayOfWeek { get; set; } = null!;
    public DateTime StartDate { get; set; }
}
