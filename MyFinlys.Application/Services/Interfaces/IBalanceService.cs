using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Application.Services.Interfaces;

public interface IBalanceService
{
    Task<BalanceDto?> GetByAccountMonthYearAsync(Guid accountId, Month month, int year);
    Task<IEnumerable<BalanceDto>> GetByAccountAsync(Guid accountId);
    Task<Guid> CreateAsync(Guid accountId, int year, Month month, decimal amount);
}
