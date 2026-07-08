using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IEventSemiAnnualService
{
    Task<IEnumerable<EventSemiAnnualDto>> GetAllAsync();
    Task<EventSemiAnnualDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(EventSemiAnnualDto dto);
    Task<EventSemiAnnualDto?> UpdateAsync(Guid id, EventSemiAnnualDto dto);
    Task<bool> DeleteAsync(Guid id);
}
