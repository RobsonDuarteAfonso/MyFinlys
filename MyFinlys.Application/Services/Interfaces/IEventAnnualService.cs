using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IEventAnnualService
{
    Task<IEnumerable<EventAnnualDto>> GetAllAsync();
    Task<EventAnnualDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(EventAnnualDto dto);
    Task<EventAnnualDto?> UpdateAsync(Guid id, EventAnnualDto dto);
    Task<bool> DeleteAsync(Guid id);
}
