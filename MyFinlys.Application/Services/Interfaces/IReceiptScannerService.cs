using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IReceiptScannerService
{
    Task<ReceiptScanResultDto?> ScanReceiptAsync(byte[] imageBytes, string contentType);
}
