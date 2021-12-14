namespace API.Infrastructure.Migrations
{
    public class PrmOtherCondition
    {
        public decimal Id { get; set; }
        public decimal ProductionInstanceId { get; set; }
        public string Criteria { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }       
    }
}
