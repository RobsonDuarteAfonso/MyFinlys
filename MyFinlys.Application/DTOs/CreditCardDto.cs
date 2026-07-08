namespace MyFinlys.Application.DTOs
{
    public class CreditCardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Limit { get; set; }
        public int ClosingDay { get; set; }
        public int DueDay { get; set; }
        public Guid? AccountId { get; set; }
        public Guid UserId { get; set; }
    }
}
