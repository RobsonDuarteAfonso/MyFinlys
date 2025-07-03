using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Application.Services;

public class EventWeeklyService : IEventWeeklyService
{
    private readonly IEventWeeklyRepository _repository;

    public EventWeeklyService(IEventWeeklyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EventWeeklyDto>> GetAllAsync()
    {
        var events = await _repository.GetAllAsync();
        return events.Select(EventWeeklyMapper.ToDto);
    }

    public async Task<EventWeeklyDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : EventWeeklyMapper.ToDto(entity);
    }

    public async Task<Guid> CreateAsync(EventWeeklyDto dto)
    {
        var installment = dto.InstallmentTotal.HasValue
            ? Installment.Create(dto.InstallmentTotal.Value, dto.InstallmentCurrent!.Value, 0, null, null)
            : null;

        var entity = new EventWeekly(
            Enum.Parse<EventType>(dto.Type),
            Enum.Parse<EventPeriod>(dto.Period),
            dto.Value,
            dto.Description,
            installment,
            Enum.Parse<Affirmation>(dto.AutoRealized),
            Enum.Parse<Affirmation>(dto.Finished),
            dto.AccountId,
            Enum.Parse<DayOfWeek>(dto.DayOfWeek)
        );

        await _repository.AddAsync(entity);
        return entity.Id;
    }
}
