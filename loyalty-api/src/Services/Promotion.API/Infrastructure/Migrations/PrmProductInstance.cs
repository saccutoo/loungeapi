using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class PrmProductInstance
    {
        public decimal Id { get; set; }
        public decimal PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public decimal ProductId { get; set; }
        public decimal ProductCopyId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductChannel { get; set; }
        public string PromotionForm { get; set; }
        public decimal LimitOnCustomer { get; set; } = 0;
        public string GiftValue { get; set; }
        public string SpendForm { get; set; }
        public string BranchScope { get; set; }
        public decimal IsNewCustomer { get; set; }
        public decimal IdentityCustomerUpload { get; set; }
        public string NotificationChannel { get; set; }
        public string NotificationImage { get; set; }
        public string NotificationLink { get; set; }
        public string Gender { get; set; }
        public string CustClass { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
