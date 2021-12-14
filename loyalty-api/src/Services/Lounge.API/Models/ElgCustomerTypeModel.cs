using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgCustomerTypeBaseModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
    }

    public class ElgCustomerTypeQueryModel : PaginationRequest
    {

    }
}
