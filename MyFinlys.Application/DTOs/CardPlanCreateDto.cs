namespace MyFinlys.Application.DTOs
{
    public class CardPlanCreateDto
    {
        public Guid CardId { get; set; }
        public Guid? AccountId { get; set; }
        public string Description { get; set; } = null!;
        public string Category { get; set; } = "Others";

        /// <summary>Valor original da compra SEM juros.</summary>
        public decimal OriginalAmount { get; set; }

        /// <summary>Valor da parcela mensal COM juros.</summary>
        public decimal MonthlyAmount { get; set; }

        public int TotalInstallments { get; set; }

        /// <summary>Parcela atual no momento do cadastro (use 1 se for novo).</summary>
        public int CurrentInstallment { get; set; } = 1;

        /// <summary>Saldo devedor atual. Se omitido, calculado como MonthlyAmount * (TotalInstallments - CurrentInstallment + 1).</summary>
        public decimal? RemainingBalance { get; set; }

        /// <summary>Dia do mês em que a parcela vence.</summary>
        public int DueDay { get; set; }

        /// <summary>Mês da primeira parcela (nome em inglês, ex: "January").</summary>
        public string StartMonth { get; set; } = null!;

        public int StartYear { get; set; }
    }
}
