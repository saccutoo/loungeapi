using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher.API.Infrastructure.Migrations
{
    public class EbankVoucher
    { 
        public decimal? VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string SerialNum { get; set; }
        public string PinNum { get; set; }
        public string VoucherDescriptionVn { get; set; }
        public string VoucherDescriptionEn { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ValueType { get; set; }
        public decimal AmountVal { get; set; }
        public decimal PercentVal { get; set; }
        public decimal MinTransAmount { get; set; }
        public decimal MaxTransAmout { get; set; }
        public string IsEligible { get; set; }
        public string Status { get; set; }
        public decimal RemainQuantity { get; set; }
        public decimal MaxQuantity { get; set; }
        public string CampName { get; set; }
        public string CampDescription { get; set; }
    }
}
