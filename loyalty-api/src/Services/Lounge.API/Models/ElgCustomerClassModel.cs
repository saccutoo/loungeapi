using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgCustomerClassBaseModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class ElgCustomerClassCreateUpdateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal OrderView { get; set; }
    }

    public class ElgCustomerClassQueryModel : PaginationRequest
    {
        public string Status { get; set; }
    }
}
