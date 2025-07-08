using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IEventMonthlyService
{
    Task<IEnumerable<EventMonthlyDto>> GetAllAsync();
    Task<EventMonthlyDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(EventMonthlyDto dto);
    Task<EventMonthlyDto?> UpdateAsync(Guid id, EventMonthlyDto dto);
    Task<bool> DeleteAsync(Guid id);
}
