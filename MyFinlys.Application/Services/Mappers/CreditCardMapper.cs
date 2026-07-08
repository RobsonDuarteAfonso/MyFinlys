using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers
{
    public static class CreditCardMapper
    {
        public static CreditCardDto ToDto(CreditCard card)
        {
            return new CreditCardDto
            {
                Id = card.Id,
                Name = card.Name,
                Limit = card.Limit,
                ClosingDay = card.ClosingDay,
                DueDay = card.DueDay,
                AccountId = card.AccountId,
                UserId = card.UserId
            };
        }
    }
}
