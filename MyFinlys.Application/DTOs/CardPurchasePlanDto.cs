namespace MyFinlys.Application.DTOs
{
    public class CardPurchasePlanDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal RemainingBalance { get; set; }
        public string Status { get; set; } = "Active"; // "Active" | "Closed"
        public int TotalInstallments { get; set; }
        public int PaidInstallments { get; set; }
        public string Category { get; set; } = null!;
        public string Type { get; set; } = "Credit";
    }
}
