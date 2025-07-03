using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;

public class EventMonthlyService : IEventMonthlyService
{
    private readonly IEventMonthlyRepository _repository;

    public EventMonthlyService(IEventMonthlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EventMonthlyDto>> GetAllAsync()
    {
        var events = await _repository.GetAllAsync();
        return events.Select(EventMonthlyMapper.ToDto);
    }

    public async Task<EventMonthlyDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : EventMonthlyMapper.ToDto(entity);
    }

    public async Task<Guid> CreateAsync(EventMonthlyDto dto)
    {
        var installment = dto.InstallmentTotal.HasValue
            ? Installment.Create(dto.InstallmentTotal.Value, dto.InstallmentCurrent!.Value, 0, null, null)
            : null;

        var entity = new EventMonthly(
            Enum.Parse<EventType>(dto.Type),
            Enum.Parse<EventPeriod>(dto.Period),
            dto.Value,
            dto.Description,
            installment,
            Enum.Parse<Affirmation>(dto.AutoRealized),
            Enum.Parse<Affirmation>(dto.Finished),
            dto.AccountId,
            dto.Due
        );

        await _repository.AddAsync(entity);
        return entity.Id;
    }
}
