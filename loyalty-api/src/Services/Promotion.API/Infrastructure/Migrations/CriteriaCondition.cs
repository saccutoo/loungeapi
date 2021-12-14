namespace API.Infrastructure.Migrations
{
    public class Criteria
    {
        public decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
    }
    public class Condition
    {
        public decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal OrderView { get; set; }
    }
    public class CriteriaCondition
    {
        public decimal Id { get; set; }
        public string CriteriaCode { get; set; }
        public string ConditionCode { get; set; }
        public string Status { get; set; } 
    }
}
