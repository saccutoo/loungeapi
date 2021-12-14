using Utils;

namespace API.Models
{
    public class StatusConfigModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Orderview { get; set; }
        public string Status { get; set; }
    }
    
    public class StatusConfigQueryModel : PaginationRequest
    {
        
    }
}
