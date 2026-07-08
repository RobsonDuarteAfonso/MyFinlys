using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email.Value,
            Avatar = user.Avatar,
            Type = (int)user.Type,
            Phone = user.Phone,
            PreferredLanguage = user.PreferredLanguage,
            Accounts = (user.UserAccounts ?? [])
                .Where(ua => ua.Account != null && ua.Account.Bank != null)
                .Select(ua => new AccountSummaryDto
                {
                    Id = ua.Account.Id,
                    Number = ua.Account.Number,
                    Type = ua.Account.Type.ToString(),
                    BankName = ua.Account.Bank.Name,
                    AccessLevel = ua.AccessLevel.ToString()
                }).ToList()
        };
    }
}
