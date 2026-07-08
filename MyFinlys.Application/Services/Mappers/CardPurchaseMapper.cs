using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Application.Services.Mappers
{
    public static class CardPurchaseMapper
    {
        public static CardPurchaseDto ToDto(CardPurchase purchase)
        {
            return new CardPurchaseDto
            {
                Id = purchase.Id,
                Description = purchase.Description,
                PurchaseDate = purchase.PurchaseDate,
                TotalAmount = purchase.TotalAmount,
                TotalInstallments = purchase.TotalInstallments,
                CardId = purchase.CardId,
                Category = purchase.Category.ToString(),
                Type = purchase.Type.ToString(),
                AccountId = purchase.AccountId,
                Installments = purchase.Installments.Select(ToInstallmentDto).ToList()
            };
        }

        public static CardInstallmentDto ToInstallmentDto(CardInstallment installment)
        {
            return new CardInstallmentDto
            {
                Id = installment.Id,
                CardPurchaseId = installment.CardPurchaseId,
                InstallmentNumber = installment.InstallmentNumber,
                Amount = installment.Amount,
                DueDate = installment.DueDate,
                Realized = installment.Realized.ToString(),
                Description = installment.CardPurchase?.Description ?? string.Empty,
                Category = installment.CardPurchase?.Category.ToString() ?? string.Empty,
                Type = installment.CardPurchase?.Type.ToString() ?? "Credit",
                TotalInstallments = installment.CardPurchase?.TotalInstallments ?? 1,
                PurchaseDate = installment.CardPurchase?.PurchaseDate ?? installment.DueDate
            };
        }
    }
}
