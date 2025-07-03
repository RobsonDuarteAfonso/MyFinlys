using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers;

public static class BankMapper
{
    public static BankDto ToDto(Bank bank)
    {
        return new BankDto
        {
            Id = bank.Id,
            Name = bank.Name
        };
    }
}
