using System;

namespace API.Infrastructure.Migrations
{
    public class ElgCustClassConfition
    {
        public decimal? Id { get; set; }
        public string ConditionUse { get; set; }
        public decimal CustClassId { get; set; }
        public string Description { get; set; }
        public string CustClassName { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal MaxCheckin { get; set; }
        public decimal MaxPeopleGoWith { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public decimal UnlimitExpireDate { set; get; }
        public bool UnlimitExpireTime { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
