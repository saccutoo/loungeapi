using Utils;

namespace API.Models
{
    public class PrmGiftModel
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
    public class PrmGiftCreateModel
    {
        public string Name { get; set; }
        public string IasCodeReference { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public string AccountName { get; set; }
        public string AccountNum { get; set; }
    }  
    public class PrmGiftQueryModel : PaginationRequest
    {
        
    }
}
