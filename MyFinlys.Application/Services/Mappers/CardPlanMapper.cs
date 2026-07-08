using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers
{
    public static class CardPlanMapper
    {
        public static CardPlanDto ToDto(CardPlan plan)
        {
            return new CardPlanDto
            {
                Id = plan.Id,
                CardId = plan.CardId,
                AccountId = plan.AccountId,
                Description = plan.Description,
                Category = plan.Category.ToString(),
                OriginalAmount = plan.OriginalAmount,
                MonthlyAmount = plan.MonthlyAmount,
                TotalInstallments = plan.TotalInstallments,
                CurrentInstallment = plan.CurrentInstallment,
                RemainingBalance = plan.RemainingBalance,
                DueDay = plan.DueDay,
                StartMonth = plan.StartMonth.ToString(),
                StartYear = plan.StartYear,
                IsActive = plan.IsActive
            };
        }
    }
}
