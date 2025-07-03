using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IBankService
{
    Task<IEnumerable<BankDto>> GetAllAsync();
    Task<BankDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(string name);
}
