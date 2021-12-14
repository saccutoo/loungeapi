using System;
using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucMapVoucherCustBaseModel : EVoucherBaseModel
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
    }

    public class VucMapVoucherCustMappingModel
    {
        public decimal VoucherId { get; set; }
        public string TransType { get; set; }
        public string UploadId { get; set; }
        public List<UsedPerCustModel> ListCustomer { get; set; }
    }

    public class UsedPerCustModel
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal MaxUsedPerCust { get; set; }
    }

    public class VucMapVoucherCustQueryModel : PaginationRequest
    {
        public string TransType { get; set; }
        public decimal VoucherId { get; set; }
        public decimal ChannelId { get; set; }
        public decimal IssueBatchId { get; set; }
        public decimal VoucherTypeId { get; set; }
    }
}
