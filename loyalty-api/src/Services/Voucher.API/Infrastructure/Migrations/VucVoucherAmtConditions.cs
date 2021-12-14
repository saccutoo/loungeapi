using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher.API.Infrastructure.Migrations
{
    public class VucVoucherAmtConditions
    {
        public decimal? Id { get; set; }
        public decimal VoucherId { get; set; }
        public decimal AmountValuation { get; set; }
        public decimal MinTransAmount { get; set; }
        public decimal MaxTransAmount { get; set; }
        public decimal PercentValuation { get; set; }
        public decimal MaxAmountCoupon { get; set; }
    }
}
