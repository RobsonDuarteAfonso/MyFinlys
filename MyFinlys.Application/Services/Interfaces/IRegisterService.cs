using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IRegisterService
{
    Task<IEnumerable<RegisterDto>> GetByEventIdAsync(Guid eventId);
    Task<IEnumerable<RegisterDto>> GetByAccountIdAsync(Guid accountId);
    Task<RegisterDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(RegisterCreateDto dto);
    Task<RegisterDto?> UpdateAsync(Guid id, RegisterUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<RegisterDto>> GetByAccountAndMonthAsync(Guid accountId, MyFinlys.Domain.Enums.Month month, int year);
    Task InitializeMonthsAsync(Guid accountId);
    Task CloseMonthAsync(Guid accountId, MyFinlys.Domain.Enums.Month month, int year);
}

