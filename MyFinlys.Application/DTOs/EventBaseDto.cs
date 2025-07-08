namespace MyFinlys.Application.DTOs;

public abstract class EventBaseDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Period { get; set; } = null!;
    public decimal Value { get; set; }
    public string Description { get; set; } = null!;
    public int? InstallmentTotal { get; set; }
    public int? InstallmentCurrent { get; set; }
    public DateTime? InstallmentDateInitial { get; set; }
    public DateTime? InstallmentDateFinish  { get; set; }    
    public string AutoRealized { get; set; } = null!;
    public string Finished { get; set; } = null!;
    public Guid AccountId { get; set; }
}
