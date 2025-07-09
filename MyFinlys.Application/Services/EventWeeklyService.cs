using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Application.Services
{
    public class EventWeeklyService : IEventWeeklyService
    {
        private readonly IEventWeeklyRepository _eventRepository;
        public EventWeeklyService(IEventWeeklyRepository eventRepository) => _eventRepository = eventRepository;

        public async Task<IEnumerable<EventWeeklyDto>> GetAllAsync() =>
            (await _eventRepository.GetAllAsync()).Select(EventWeeklyMapper.ToDto);

        public async Task<EventWeeklyDto?> GetByIdAsync(Guid id) =>
            (await _eventRepository.GetByIdAsync(id)) is EventWeekly e
                ? EventWeeklyMapper.ToDto(e)
                : null;

        public async Task<Guid> CreateAsync(EventWeeklyDto dto)
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

            var entity = new EventWeekly(
                Enum.Parse<EventType>(dto.Type, true),
                Enum.Parse<EventPeriod>(dto.Period, true),
                dto.Value,
                dto.Description,
                inst,
                Enum.Parse<Affirmation>(dto.AutoRealized, true),
                Enum.Parse<Affirmation>(dto.Finished, true),
                dto.AccountId,
                Enum.Parse<DayOfWeek>(dto.DayOfWeek, true)
            );

            await _eventRepository.AddAsync(entity);
            await _eventRepository.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<EventWeeklyDto?> UpdateAsync(Guid id, EventWeeklyDto dto)
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
                Enum.Parse<DayOfWeek>(dto.DayOfWeek, true)
            );

            await _eventRepository.UpdateAsync(entity);
            await _eventRepository.SaveChangesAsync();
            return EventWeeklyMapper.ToDto(entity);
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
