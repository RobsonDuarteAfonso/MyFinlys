using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Application.Services.Interfaces;

public interface IBalanceService
{
    Task<BalanceDto?> GetByAccountMonthYearAsync(Guid accountId, Month month, int year);
    Task<IEnumerable<BalanceDto>> GetByAccountAsync(Guid accountId);
    Task<PaginatedResult<BalanceDto>> GetByAccountPaginatedAsync(Guid accountId, PaginationParams @params);
    Task<Guid> CreateAsync(Guid accountId, int year, Month month, decimal amount);
    Task<BalanceDto?> UpdateAsync(Guid id, decimal amount);
    Task<bool> DeleteAsync(Guid id);
}
