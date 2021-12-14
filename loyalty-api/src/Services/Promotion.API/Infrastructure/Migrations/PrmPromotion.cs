using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class PrmPromotion
    {
        public decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? IsRegisterIAT { get; set; }
        public decimal? LimitOnPromotion { get; set; }
        public decimal? LimitOnCustomer { get; set; }
        public decimal? CumulativeValue { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public string ApprovedComment { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
