namespace MyFinlys.Application.DTOs
{
    public class CardPlanDto
    {
        public Guid Id { get; set; }
        public Guid CardId { get; set; }
        public Guid? AccountId { get; set; }
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal OriginalAmount { get; set; }
        public decimal MonthlyAmount { get; set; }
        public int TotalInstallments { get; set; }
        public int CurrentInstallment { get; set; }
        public decimal RemainingBalance { get; set; }
        public int DueDay { get; set; }
        public string StartMonth { get; set; } = null!;
        public int StartYear { get; set; }
        public bool IsActive { get; set; }
        public string Status => IsActive ? "Active" : "Closed";
    }
}
