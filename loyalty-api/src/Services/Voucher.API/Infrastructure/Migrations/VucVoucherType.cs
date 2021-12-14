using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher.API.Infrastructure.Migrations
{
    public class VucVoucherType
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string ValueType { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
