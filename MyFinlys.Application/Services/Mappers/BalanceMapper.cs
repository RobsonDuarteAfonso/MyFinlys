using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class BalanceMapper
{
    public static BalanceDto ToDto(Balance balance)
    {
        return new BalanceDto
        {
            Id = balance.Id,
            Year = balance.Year,
            Month = balance.Month.ToString(),
            Amount = balance.Amount,
            AccountId = balance.AccountId
        };
    }
}
