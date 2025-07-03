using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;

public class EventBiweeklyService : IEventBiweeklyService
{
    private readonly IEventBiweeklyRepository _repository;

    public EventBiweeklyService(IEventBiweeklyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EventBiweeklyDto>> GetAllAsync()
    {
        var events = await _repository.GetAllAsync();
        return events.Select(EventBiweeklyMapper.ToDto);
    }

    public async Task<EventBiweeklyDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : EventBiweeklyMapper.ToDto(entity);
    }

    public async Task<Guid> CreateAsync(EventBiweeklyDto dto)
    {
        var installment = dto.InstallmentTotal.HasValue
            ? Installment.Create(dto.InstallmentTotal.Value, dto.InstallmentCurrent!.Value, 0, null, null)
            : null;

        var entity = new EventBiweekly(
            Enum.Parse<EventType>(dto.Type),
            Enum.Parse<EventPeriod>(dto.Period),
            dto.Value,
            dto.Description,
            installment,
            Enum.Parse<Affirmation>(dto.AutoRealized),
            Enum.Parse<Affirmation>(dto.Finished),
            dto.AccountId,
            Enum.Parse<DayOfWeek>(dto.DayOfWeek),
            dto.StartDate
        );

        await _repository.AddAsync(entity);
        return entity.Id;
    }
}
