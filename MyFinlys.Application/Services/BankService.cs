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
        return bank is null
            ? null
            : BankMapper.ToDto(bank);
    }

    public async Task<Guid> CreateAsync(BankCreateDto dto)
    {
        var entity = Bank.Create(dto.Name);
        await _bankRepository.AddAsync(entity);
        await _bankRepository.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<BankDto?> UpdateAsync(Guid id, BankUpdateDto dto)
    {
        var bank = await _bankRepository.GetByIdAsync(id);
        if (bank is null)
            return null;

        bank.Update(dto.Name);
        await _bankRepository.UpdateAsync(bank);
        await _bankRepository.SaveChangesAsync();
        return BankMapper.ToDto(bank);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var bank = await _bankRepository.GetByIdAsync(id);
        if (bank is null)
            return false;

        await _bankRepository.DeleteAsync(id);
        await _bankRepository.SaveChangesAsync();
        return true;
    }
}
