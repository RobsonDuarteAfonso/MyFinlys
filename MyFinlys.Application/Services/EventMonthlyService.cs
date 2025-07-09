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
        private readonly IEventMonthlyRepository _eventRepository;
        public EventMonthlyService(IEventMonthlyRepository eventRepository) => _eventRepository = eventRepository;

        public async Task<IEnumerable<EventMonthlyDto>> GetAllAsync() =>
            (await _eventRepository.GetAllAsync()).Select(EventMonthlyMapper.ToDto);

        public async Task<EventMonthlyDto?> GetByIdAsync(Guid id) =>
            (await _eventRepository.GetByIdAsync(id)) is EventMonthly e
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

            await _eventRepository.AddAsync(entity);
            await _eventRepository.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<EventMonthlyDto?> UpdateAsync(Guid id, EventMonthlyDto dto)
        {
            var entity = await _eventRepository.GetByIdAsync(id);
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

            await _eventRepository.UpdateAsync(entity);
            await _eventRepository.SaveChangesAsync();
            return EventMonthlyMapper.ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (await _eventRepository.GetByIdAsync(id) is null)
                return false;

            await _eventRepository.DeleteAsync(id);
            await _eventRepository.SaveChangesAsync();
            return true;
        }
    }
}
