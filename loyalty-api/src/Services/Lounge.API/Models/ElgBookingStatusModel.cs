using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgBookingStatusBaseModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
    }

    public class ElgBookingStatusQueryModel : PaginationRequest
    {

    }
}
