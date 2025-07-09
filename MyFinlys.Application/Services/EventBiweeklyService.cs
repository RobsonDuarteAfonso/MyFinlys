using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Application.Services
{
    public class EventBiweeklyService : IEventBiweeklyService
    {
        private readonly IEventBiweeklyRepository _eventRepository;
        public EventBiweeklyService(IEventBiweeklyRepository eventRepository) => _eventRepository = eventRepository;

        public async Task<IEnumerable<EventBiweeklyDto>> GetAllAsync() =>
            (await _eventRepository.GetAllAsync()).Select(EventBiweeklyMapper.ToDto);

        public async Task<EventBiweeklyDto?> GetByIdAsync(Guid id) =>
            (await _eventRepository.GetByIdAsync(id)) is EventBiweekly e
                ? EventBiweeklyMapper.ToDto(e)
                : null;

        public async Task<Guid> CreateAsync(EventBiweeklyDto dto)
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

            var entity = new EventBiweekly(
                Enum.Parse<EventType>(dto.Type, true),
                Enum.Parse<EventPeriod>(dto.Period, true),
                dto.Value,
                dto.Description,
                inst,
                Enum.Parse<Affirmation>(dto.AutoRealized, true),
                Enum.Parse<Affirmation>(dto.Finished, true),
                dto.AccountId,
                Enum.Parse<DayOfWeek>(dto.DayOfWeek, true),
                dto.StartDate
            );

            await _eventRepository.AddAsync(entity);
            await _eventRepository.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<EventBiweeklyDto?> UpdateAsync(Guid id, EventBiweeklyDto dto)
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
                Enum.Parse<DayOfWeek>(dto.DayOfWeek, true),
                dto.StartDate
            );

            await _eventRepository.UpdateAsync(entity);
            await _eventRepository.SaveChangesAsync();
            return EventBiweeklyMapper.ToDto(entity);
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
