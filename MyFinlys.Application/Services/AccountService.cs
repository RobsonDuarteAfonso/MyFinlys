using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankRepository _bankRepository;
    private readonly IUserRepository _userRepository;

    public AccountService(
        IAccountRepository accountRepo,
        IBankRepository bankRepo,
        IUserRepository userRepo)
    {
        _accountRepository = accountRepo;
        _bankRepository = bankRepo;
        _userRepository = userRepo;
    }

    public async Task<AccountDetailDto?> GetByIdAsync(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account is null ? null : AccountMapper.ToDetailDto(account);
    }

    public async Task<AccountDetailDto?> GetByNumberAsync(string number)
    {
        var account = await _accountRepository.GetByNumberAsync(number);
        return account is null ? null : AccountMapper.ToDetailDto(account);
    }

    public async Task<IEnumerable<AccountSummaryDto>> GetByUserIdAsync(Guid userId)
    {
        var accounts = await _accountRepository.GetByUserIdAsync(userId);
        return accounts.Select(AccountMapper.ToSummaryDto);
    }

    public async Task<IEnumerable<AccountSummaryDto>> GetAllAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(AccountMapper.ToSummaryDto);
    }

    public async Task<AccountDetailDto> CreateAsync(AccountCreateDto dto)
    {
        if (!Enum.TryParse<AccountType>(dto.Type, true, out var parsedType))
            throw new ArgumentException("Invalid account type.", nameof(dto.Type));

        var bank = await _bankRepository.GetByIdAsync(dto.BankId)
                   ?? throw new ArgumentException("Bank not found.", nameof(dto.BankId));

        var account = Account.Create(dto.Number, parsedType, bank.Id);

        foreach (var userId in dto.UserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new ArgumentException($"User not found: {userId}");
            account.AddUser(user);
        }

        await _accountRepository.AddAsync(account);
        return AccountMapper.ToDetailDto(account);
    }

    public async Task<AccountDetailDto?> UpdateAsync(Guid id, AccountUpdateDto dto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account is null)
            return null;

        if (!Enum.TryParse<AccountType>(dto.Type, true, out var parsedType))
            throw new ArgumentException("Invalid account type.", nameof(dto.Type));

        account.Update(dto.Number, parsedType);
        await _accountRepository.UpdateAsync(account);

        return AccountMapper.ToDetailDto(account);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _accountRepository.GetByIdAsync(id);
        if (existing is null)
            return false;

        await _accountRepository.DeleteAsync(id);
        return true;
    }
}
