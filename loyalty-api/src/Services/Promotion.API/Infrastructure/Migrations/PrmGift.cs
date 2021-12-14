namespace API.Infrastructure.Migrations
{
    public class PrmGift
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public decimal ProductionInstanceId { get; set; }
        public string IasCodeReference { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Donated { get; set; }
        public decimal CostPrice { get; set; }
        public string AccountName { get; set; }
        public string AccountNum { get; set; }
    }
}
