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

    public BalanceService(IBalanceRepository balanceRepo, IAccountRepository accountRepo)
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

    public async Task<Guid> CreateAsync(Guid accountId, int year, Month month, decimal amount)
    {
        var yearVO = Year.Create(year);
        var balance = Balance.Create(accountId, yearVO, month, amount);
        await _balanceRepository.AddAsync(balance);
        return balance.Id;
    }
}
