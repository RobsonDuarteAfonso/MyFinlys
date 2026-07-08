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
                Email = ua.User.Email.Value,
                AccessLevel = ua.AccessLevel.ToString()
            }).ToList()
        };
    }

    public static AccountSummaryDto ToSummaryDto(Account account, Guid? userId = null)
    {
        var accessLevel = string.Empty;
        if (userId.HasValue)
        {
            var ua = account.UserAccounts.FirstOrDefault(x => x.UserId == userId.Value);
            if (ua != null)
            {
                accessLevel = ua.AccessLevel.ToString();
            }
        }
        else
        {
            var ua = account.UserAccounts.FirstOrDefault();
            if (ua != null)
            {
                accessLevel = ua.AccessLevel.ToString();
            }
        }

        return new AccountSummaryDto
        {
            Id = account.Id,
            Number = account.Number,
            Type = account.Type.ToString(),
            BankName = account.Bank.Name,
            AccessLevel = accessLevel
        };
    }
}
