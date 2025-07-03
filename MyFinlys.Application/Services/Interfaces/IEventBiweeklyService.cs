using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IEventBiweeklyService
{
    Task<IEnumerable<EventBiweeklyDto>> GetAllAsync();
    Task<EventBiweeklyDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(EventBiweeklyDto dto);
}
