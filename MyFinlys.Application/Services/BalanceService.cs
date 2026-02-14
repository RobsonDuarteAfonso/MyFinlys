// MyFinlys.Application/Services/BalanceService.cs
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly IBalanceRepository _balanceRepository;
    private readonly IAccountRepository _accountRepository;

    public BalanceService(
        IBalanceRepository balanceRepo,
        IAccountRepository accountRepo)
    {
        _balanceRepository = balanceRepo;
        _accountRepository = accountRepo;
    }

    public async Task<BalanceDto?> GetByAccountMonthYearAsync(Guid accountId, Month month, int year)
    {
        var yearVO = Year.Create(year);
        var balance = await _balanceRepository.GetByAccountMonthYearAsync(accountId, month, yearVO);
        return balance is null ? null : BalanceMapper.ToDto(balance);
    }

    public async Task<IEnumerable<BalanceDto>> GetByAccountAsync(Guid accountId)
    {
        var balances = await _balanceRepository.GetByAccountAsync(accountId);
        return balances.Select(BalanceMapper.ToDto);
    }

    public async Task<PaginatedResult<BalanceDto>> GetByAccountPaginatedAsync(Guid accountId, PaginationParams @params)
    {
        @params.Validate();
        var balances = await _balanceRepository.GetByAccountAsync(accountId);
        var filteredBalances = balances.Where(b => b.AccountId == accountId).ToList();
        var totalCount = filteredBalances.Count;
        var items = filteredBalances
            .Skip((@params.PageNumber - 1) * @params.PageSize)
            .Take(@params.PageSize)
            .Select(BalanceMapper.ToDto)
            .ToList();

        return new PaginatedResult<BalanceDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = @params.PageNumber,
            PageSize = @params.PageSize
        };
    }

    public async Task<Guid> CreateAsync(Guid accountId, int year, Month month, decimal amount)
    {

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
            throw new ArgumentException($"Account not found: {accountId}", nameof(accountId));

        var yearVO = Year.Create(year);
        var balance = Balance.Create(accountId, yearVO, month, amount);

        await _balanceRepository.AddAsync(balance);
        await _balanceRepository.SaveChangesAsync();
        return balance.Id;
    }

    public async Task<BalanceDto?> UpdateAsync(Guid id, decimal amount)
    {
        var balance = await _balanceRepository.GetByIdAsync(id);
        if (balance is null) return null;

        var account = await _accountRepository.GetByIdAsync(balance.AccountId);
        if (account is null)
            throw new InvalidOperationException($"Associated account not found: {balance.AccountId}");

        balance.UpdateAmount(amount);
        balance.SetUpdatedAt();

        await _balanceRepository.UpdateAsync(balance);
        await _balanceRepository.SaveChangesAsync();
        return BalanceMapper.ToDto(balance);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _balanceRepository.GetByIdAsync(id);
        if (existing is null) return false;

        await _balanceRepository.DeleteAsync(id);
        await _balanceRepository.SaveChangesAsync();
        return true;
    }
}
