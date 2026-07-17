namespace MyFinlys.Application.DTOs;

public class ReceiptScanResultDto
{
    public decimal Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Due { get; set; }
    public string Category { get; set; } = string.Empty;
}
