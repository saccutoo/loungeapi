using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucVoucherAmtConditionsBaseModel
    {
        public decimal? Id { get; set; }
        public decimal VoucherId { get; set; }
        public decimal AmountValuation { get; set; }
        public decimal MinTransAmount { get; set; }
        public decimal MaxTransAmount { get; set; }
        public decimal PercentValuation { get; set; }
        public decimal MaxAmountCoupon { get; set; }
    }

    public class VucVoucherAmtConditionsCreateModel
    {
        public decimal VoucherId { get; set; }
        public decimal AmountValuation { get; set; }
        public decimal MinTransAmount { get; set; }
        public decimal MaxTransAmount { get; set; }
        public decimal PercentValuation { get; set; }
        public decimal MaxAmountCoupon { get; set; }
    }

    public class VucVoucherAmtConditionsQueryModel : PaginationRequest
    {
        public string ChannelId { get; set; }
        public string IssueBatchId { get; set; }
        public string VoucherType { get; set; }
        public string Status { get; set; }
    }
}
