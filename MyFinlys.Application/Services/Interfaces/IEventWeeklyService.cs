using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IEventWeeklyService
{
    Task<IEnumerable<EventWeeklyDto>> GetAllAsync();
    Task<EventWeeklyDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(EventWeeklyDto dto);
    Task<EventWeeklyDto?> UpdateAsync(Guid id, EventWeeklyDto dto);
    Task<bool> DeleteAsync(Guid id);
}
