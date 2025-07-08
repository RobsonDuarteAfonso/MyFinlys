using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Application.Services
{
    public class EventMonthlyService : IEventMonthlyService
    {
        private readonly IEventMonthlyRepository _repo;
        public EventMonthlyService(IEventMonthlyRepository repo) => _repo = repo;

        public async Task<IEnumerable<EventMonthlyDto>> GetAllAsync() =>
            (await _repo.GetAllAsync()).Select(EventMonthlyMapper.ToDto);

        public async Task<EventMonthlyDto?> GetByIdAsync(Guid id) =>
            (await _repo.GetByIdAsync(id)) is EventMonthly e
                ? EventMonthlyMapper.ToDto(e)
                : null;

        public async Task<Guid> CreateAsync(EventMonthlyDto dto)
        {
            Installment? inst = null;
            if (dto.InstallmentTotal.HasValue && dto.InstallmentCurrent.HasValue)
            {
                inst = Installment.Create(
                    dto.InstallmentTotal.Value,
                    dto.InstallmentCurrent.Value,
                    dto.Value,
                    dto.InstallmentDateInitial,
                    dto.InstallmentDateFinish
                );
            }

            var entity = new EventMonthly(
                Enum.Parse<EventType>(dto.Type, true),
                Enum.Parse<EventPeriod>(dto.Period, true),
                dto.Value,
                dto.Description,
                inst,
                Enum.Parse<Affirmation>(dto.AutoRealized, true),
                Enum.Parse<Affirmation>(dto.Finished, true),
                dto.AccountId,
                dto.Due
            );

            await _repo.AddAsync(entity);
            return entity.Id;
        }

        public async Task<EventMonthlyDto?> UpdateAsync(Guid id, EventMonthlyDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            Installment? inst = null;
            if (dto.InstallmentTotal.HasValue && dto.InstallmentCurrent.HasValue)
            {
                inst = Installment.Create(
                    dto.InstallmentTotal.Value,
                    dto.InstallmentCurrent.Value,
                    dto.Value,
                    dto.InstallmentDateInitial,
                    dto.InstallmentDateFinish
                );
            }

            entity.Update(
                Enum.Parse<EventType>(dto.Type, true),
                Enum.Parse<EventPeriod>(dto.Period, true),
                dto.Value,
                dto.Description,
                inst,
                Enum.Parse<Affirmation>(dto.AutoRealized, true),
                Enum.Parse<Affirmation>(dto.Finished, true),
                dto.AccountId,
                dto.Due
            );

            await _repo.UpdateAsync(entity);
            return EventMonthlyMapper.ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (await _repo.GetByIdAsync(id) is null)
                return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
