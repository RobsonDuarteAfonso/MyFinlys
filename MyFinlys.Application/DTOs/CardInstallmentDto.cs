namespace MyFinlys.Application.DTOs
{
    public class CardInstallmentDto
    {
        public Guid Id { get; set; }
        public Guid CardPurchaseId { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Realized { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Type { get; set; } = "Credit";
        public int TotalInstallments { get; set; }
    }
}
