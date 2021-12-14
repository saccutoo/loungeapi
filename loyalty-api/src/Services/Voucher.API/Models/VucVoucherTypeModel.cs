using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucVoucherTypeBaseModel : EVoucherBaseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string ValueType { get; set; }
    }

    public class VucVoucherTypeQueryModel : PaginationRequest
    {

    }
}
