using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IEventQuarterlyService
{
    Task<IEnumerable<EventQuarterlyDto>> GetAllAsync();
    Task<EventQuarterlyDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(EventQuarterlyDto dto);
    Task<EventQuarterlyDto?> UpdateAsync(Guid id, EventQuarterlyDto dto);
    Task<bool> DeleteAsync(Guid id);
}
