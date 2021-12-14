using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucAppliedChannelBaseModel : EVoucherBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ChannelCode { get; set; }
        public string Status { get; set; }
        public decimal OrderView { get; set; }
    }

    public class VucAppliedChannelQueryModel : PaginationRequest
    {

    }
}
