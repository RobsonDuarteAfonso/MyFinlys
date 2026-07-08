namespace MyFinlys.Application.DTOs
{
    public class CardPurchaseCreateDto
    {
        public string Description { get; set; } = null!;
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalInstallments { get; set; }
        public Guid CardId { get; set; }
        public string Category { get; set; } = null!;
        public string Type { get; set; } = "Credit";
        public int ClosingDay { get; set; }
        public Guid? AccountId { get; set; }
    }
}
