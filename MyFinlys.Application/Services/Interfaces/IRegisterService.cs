using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IRegisterService
{
    Task<IEnumerable<RegisterDto>> GetByEventIdAsync(Guid eventId);
    Task<RegisterDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(RegisterCreateDto dto);
    Task<RegisterDto?> UpdateAsync(Guid id, RegisterUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
