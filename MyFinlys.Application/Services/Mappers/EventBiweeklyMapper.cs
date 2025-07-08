using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class EventBiweeklyMapper
{
    public static EventBiweeklyDto ToDto(EventBiweekly e)
    {
        return new EventBiweeklyDto
        {
            Id = e.Id,
            Type = e.Type.ToString(),
            Period = e.Period.ToString(),
            Value = e.Value,
            Description = e.Description,
            InstallmentTotal = e.Installment?.InstallmentTotal,
            InstallmentCurrent = e.Installment?.InstallmentCurrent,
            InstallmentDateInitial = e.Installment?.DateInitial,
            InstallmentDateFinish  = e.Installment?.DateFinish,
            AutoRealized = e.AutoRealized.ToString(),
            Finished = e.Finished.ToString(),
            AccountId = e.AccountId,
            DayOfWeek = e.DayOfWeek.ToString(),
            StartDate = e.StartDate
        };
    }
}
