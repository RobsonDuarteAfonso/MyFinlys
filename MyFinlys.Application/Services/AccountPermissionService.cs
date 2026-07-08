using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services;

public class AccountPermissionService : IAccountPermissionService
{
    private readonly IAccountRepository _accountRepository;

    public AccountPermissionService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<bool> HasAccessAsync(Guid userId, Guid accountId, params AccessLevel[] allowedLevels)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null) return false;

        var userAccount = account.UserAccounts.FirstOrDefault(ua => ua.UserId == userId);
        if (userAccount == null) return false;

        return allowedLevels.Contains(userAccount.AccessLevel);
    }

    public async Task<bool> CanModifyUsersAsync(Guid userId, Guid accountId)
    {
        return await HasAccessAsync(userId, accountId, AccessLevel.Owner);
    }
}
