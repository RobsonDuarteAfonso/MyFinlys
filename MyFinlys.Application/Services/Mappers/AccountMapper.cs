using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class AccountMapper
{
    public static AccountDetailDto ToDetailDto(Account account)
    {
        return new AccountDetailDto
        {
            Id = account.Id,
            Number = account.Number,
            Type = account.Type.ToString(),
            BankName = account.Bank.Name,
            Users = account.UserAccounts.Select(ua => new UserBasicDto
            {
                Id = ua.User.Id,
                Name = ua.User.Name,
                Email = ua.User.Email.Value
            }).ToList()
        };
    }

    public static AccountSummaryDto ToSummaryDto(Account account)
    {
        return new AccountSummaryDto
        {
            Id = account.Id,
            Number = account.Number,
            Type = account.Type.ToString(),
            BankName = account.Bank.Name
        };
    }
}
