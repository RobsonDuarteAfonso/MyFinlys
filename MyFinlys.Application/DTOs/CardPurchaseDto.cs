namespace MyFinlys.Application.DTOs
{
    public class CardPurchaseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalInstallments { get; set; }
        public Guid CardId { get; set; }
        public string Category { get; set; } = null!;
        public string Type { get; set; } = "Credit";
        public Guid? AccountId { get; set; }
        public List<CardInstallmentDto> Installments { get; set; } = [];
    }
}
