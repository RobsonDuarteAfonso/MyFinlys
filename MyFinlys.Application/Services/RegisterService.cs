using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services;

public class RegisterService : IRegisterService
{
    private readonly IRegisterRepository _repository;

    public RegisterService(IRegisterRepository repository)
    {
        _repository = repository;
    }

    public async Task<RegisterDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : RegisterMapper.ToDto(entity);
    }

    public async Task<IEnumerable<RegisterDto>> GetByEventIdAsync(Guid eventId)
    {
        var items = await _repository.GetByEventIdAsync(eventId);
        return items.Select(RegisterMapper.ToDto);
    }

    public async Task<Guid> CreateAsync(RegisterCreateDto dto)
    {
        var entity = Register.Create(
            dto.Due,
            Enum.Parse<EventType>(dto.EventType),
            dto.InstallmentCurrent,
            dto.Value,
            dto.Subdescription,
            Enum.Parse<Month>(dto.Month),
            dto.Week,
            Enum.Parse<Affirmation>(dto.Realized),
            dto.EventId
        );

        await _repository.AddAsync(entity);
        return entity.Id;
    }

    public async Task<RegisterDto?> UpdateAsync(Guid id, RegisterUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return null;

        entity.Update(
            dto.Due,
            Enum.Parse<EventType>(dto.EventType),
            dto.InstallmentCurrent,
            dto.Value,
            dto.Subdescription,
            Enum.Parse<Month>(dto.Month),
            dto.Week,
            Enum.Parse<Affirmation>(dto.Realized),
            dto.EventId
        );

        await _repository.UpdateAsync(entity);
        return RegisterMapper.ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        await _repository.DeleteAsync(entity.Id);
        return true;
    }

}
