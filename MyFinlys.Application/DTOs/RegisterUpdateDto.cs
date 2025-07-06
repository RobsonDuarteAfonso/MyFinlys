namespace MyFinlys.Application.DTOs;

public class RegisterUpdateDto
{
    public DateTime Due { get; set; }
    public string EventType { get; set; } = default!;
    public int InstallmentCurrent { get; set; }
    public decimal Value { get; set; }
    public string Subdescription { get; set; } = default!;
    public string Month { get; set; } = default!;
    public int Week { get; set; }
    public string Realized { get; set; } = default!;
    public Guid EventId { get; set; }
}
