using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class PrmCostAccount
    {
        public decimal Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string GiftForm { get; set; }
        public string CustomerType { get; set; }
        public decimal IsRegisterIAT { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public decimal OrderView { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
