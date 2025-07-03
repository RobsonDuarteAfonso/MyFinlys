using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class EventWeeklyMapper
{
    public static EventWeeklyDto ToDto(EventWeekly e)
    {
        return new EventWeeklyDto
        {
            Id = e.Id,
            Type = e.Type.ToString(),
            Period = e.Period.ToString(),
            Value = e.Value,
            Description = e.Description,
            InstallmentTotal = e.Installment?.InstallmentTotal,
            InstallmentCurrent = e.Installment?.InstallmentCurrent,
            AutoRealized = e.AutoRealized.ToString(),
            Finished = e.Finished.ToString(),
            AccountId = e.AccountId,
            DayOfWeek = e.DayOfWeek.ToString()
        };
    }
}
