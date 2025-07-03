using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IRegisterService
{
    Task<IEnumerable<RegisterDto>> GetByEventIdAsync(Guid eventId);
    Task<RegisterDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(RegisterDto dto);
}
