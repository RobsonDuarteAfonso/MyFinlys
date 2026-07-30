using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class EventSemiAnnualMapper
{
    public static EventSemiAnnualDto ToDto(EventSemiAnnual e)
    {
        return new EventSemiAnnualDto
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
            Due = e.Due,
            Category = e.Category.ToString(),
            EndDate = e.EndDate,
            CreditCardId = e.CreditCardId,
            CreditCardName = e.CreditCard?.Name
        };
    }
}
