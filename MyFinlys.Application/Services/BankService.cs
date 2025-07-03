using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services;

public class BankService : IBankService
{
    private readonly IBankRepository _bankRepository;

    public BankService(IBankRepository bankRepository)
    {
        _bankRepository = bankRepository;
    }

    public async Task<IEnumerable<BankDto>> GetAllAsync()
    {
        var banks = await _bankRepository.GetAllAsync();
        return banks.Select(BankMapper.ToDto);
    }

    public async Task<BankDto?> GetByIdAsync(Guid id)
    {
        var bank = await _bankRepository.GetByIdAsync(id);
        return bank is null ? null : BankMapper.ToDto(bank);
    }

    public async Task<Guid> CreateAsync(string name)
    {
        var bank = Bank.Create(name);
        await _bankRepository.AddAsync(bank);
        return bank.Id;
    }
}
