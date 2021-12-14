using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher.API.Infrastructure.Migrations
{
    public class VucVoucher
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string DescriptionVn { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal IssueBatchId { get; set; }
        public decimal ChannelId { get; set; }
        public string ChannelName { get; set; }
        public decimal MaxUsedQuantity { get; set; }
        public decimal IssueQuantity { get; set; }
        public string Status { get; set; }
        public string Theme { get; set; }
        public decimal VoucherTypeId { get; set; }
        public decimal RemainQuantity { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
