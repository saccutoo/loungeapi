namespace API.Infrastructure.Migrations
{
    public class PrmTransactionCondition
    {
        public decimal Id { get; set; }
        public decimal ProductionInstanceId { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal FromTerm { get; set; }
        public decimal ToTerm { get; set; }
        public decimal CashValue { get; set; }
        public decimal PercentValue { get; set; }        
    }
}
