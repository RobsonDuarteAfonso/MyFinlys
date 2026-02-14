using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IAccountService
{
    Task<AccountDetailDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AccountSummaryDto>> GetByUserIdAsync(Guid userId);
    Task<AccountDetailDto?> GetByNumberAsync(string number);
    Task<IEnumerable<AccountSummaryDto>> GetAllAsync();
    Task<PaginatedResult<AccountSummaryDto>> GetAllPaginatedAsync(PaginationParams @params);
    Task<AccountDetailDto> CreateAsync(AccountCreateDto dto);
    Task<AccountDetailDto?> UpdateAsync(Guid id, AccountUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
