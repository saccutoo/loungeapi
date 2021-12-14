using System;
using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucVoucherBaseModel : EVoucherBaseModel
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
    }

    public class VucVoucherCreateUpdateModel
    {
        public string Name { get; set; }
        public string DescriptionVn { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal IssueBatchId { get; set; }
        public decimal ChannelId { get; set; }
        public decimal IssueQuantity { get; set; }
        public decimal MaxUsedQuantity { get; set; }
        public string Theme { get; set; }
        public decimal VoucherTypeId { get; set; }
        public List<VucVoucherAmtConditionsCreateModel> ListCondition { get; set; }
    }

    public class VucVoucherQueryModel : PaginationRequest
    {
        public decimal ChannelId { get; set; }
        public decimal IssueBatchId { get; set; }
        public string VoucherType { get; set; }
        public decimal StatusId { get; set; }
    }
}
