using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucTransTypeBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TransType { get; set; }
    }

    public class VucTransTypeQueryModel : PaginationRequest
    {

    }
}
