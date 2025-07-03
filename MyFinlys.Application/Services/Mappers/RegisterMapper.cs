using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class RegisterMapper
{
    public static RegisterDto ToDto(Register register)
    {
        return new RegisterDto
        {
            Id = register.Id,
            Due = register.Due,
            EventType = register.EventType.ToString(),
            InstallmentCurrent = register.InstallmentCurrent,
            Value = register.Value,
            Subdescription = register.Subdescription,
            Month = register.Month.ToString(),
            Week = register.Week,
            Realized = register.Realized.ToString(),
            EventId = register.EventId
        };
    }
}
