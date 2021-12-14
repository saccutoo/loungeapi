using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher.API.Infrastructure.Migrations
{
    public class VucMapVoucherCust
    {
        public decimal? Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal MaxUsedQuantityPerCust { get; set; }
        public decimal CountUsed { get; set; }
        public decimal PublishVoucherId { get; set; }
        public string SerialNum { get; set; }
        public string Status { get; set; }
        public string TransType { get; set; }
        public decimal UploadId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Name { get; set; }
        public string ChannelName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
