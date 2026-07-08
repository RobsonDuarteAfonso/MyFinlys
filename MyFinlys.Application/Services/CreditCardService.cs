using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services
{
    public class CreditCardService : ICreditCardService
    {
        private readonly ICreditCardRepository _repository;
        private readonly IAccountRepository _accountRepository;

        public CreditCardService(
            ICreditCardRepository repository,
            IAccountRepository accountRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
        }

        public async Task<CreditCardDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity is null ? null : CreditCardMapper.ToDto(entity);
        }

        public async Task<IEnumerable<CreditCardDto>> GetByAccountAsync(Guid accountId)
        {
            var items = await _repository.GetByAccountAsync(accountId);
            return items.Select(CreditCardMapper.ToDto);
        }

        public async Task<IEnumerable<CreditCardDto>> GetByUserIdAsync(Guid userId)
        {
            var items = await _repository.GetByUserIdAsync(userId);
            return items.Select(CreditCardMapper.ToDto);
        }

        public async Task<Guid> CreateAsync(CreditCardCreateDto dto, Guid userId)
        {
            if (dto.AccountId.HasValue)
            {
                var account = await _accountRepository.GetByIdAsync(dto.AccountId.Value);
                if (account is null)
                    throw new ArgumentException("Account not found.", nameof(dto.AccountId));
            }

            var entity = CreditCard.Create(
                dto.Name,
                dto.Limit,
                dto.ClosingDay,
                dto.DueDay,
                dto.AccountId,
                userId
            );

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<CreditCardDto?> UpdateAsync(Guid id, CreditCardUpdateDto dto, Guid userId)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null) return null;

            if (dto.AccountId.HasValue)
            {
                var account = await _accountRepository.GetByIdAsync(dto.AccountId.Value);
                if (account is null)
                    throw new ArgumentException("Account not found.", nameof(dto.AccountId));
            }

            entity.Update(
                dto.Name,
                dto.Limit,
                dto.ClosingDay,
                dto.DueDay,
                dto.AccountId,
                userId
            );

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return CreditCardMapper.ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null) return false;

            entity.SoftDelete();
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
