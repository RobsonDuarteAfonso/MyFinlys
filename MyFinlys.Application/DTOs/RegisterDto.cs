namespace MyFinlys.Application.DTOs;

public class RegisterDto
{
    public Guid Id { get; set; }
    public DateTime Due { get; set; }
    public string EventType { get; set; } = null!;
    public int InstallmentCurrent { get; set; }
    public decimal Value { get; set; }
    public string Subdescription { get; set; } = null!;
    public string Month { get; set; } = null!;
    public int Week { get; set; }
    public string Realized { get; set; } = null!;
    public Guid EventId { get; set; }
}
