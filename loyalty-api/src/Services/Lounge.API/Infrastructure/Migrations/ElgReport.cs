using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class ElgReport
    {
        public decimal? Id { get; set; }
        public DateTime BirthDay { get; set; }
        public decimal ClassId { get; set; }
        public string CustId { get; set; }
        public string CifNum { get; set; }
        public decimal CustTypeId { get; set; }
        public string Email { get; set; }
        public DateTime ExpireDate { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string PhoneNum { get; set; }
        public decimal PosId { get; set; }
        public string RepresentUserId { get; set; }
        public string RepresentUserName { get; set; }
        public decimal UploadId { get; set; }
        public string CustClassName { get; set; }
        public string CustTypeName { get; set; }
        public string BranchName { get; set; }
        public string PosName { get; set; }
        public decimal NumOfUse { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public DateTime BookingTime { get; set; }        
        public string GoWithCustId { get; set; }
        public string GoWithName { get; set; }
        public string GoWithPhoneNum { get; set; }
        public decimal TotalBuyMore { get; set; }
        public decimal TotalUsedBSV { get; set; }
        public string BSVClassName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
        public DateTime? LastCheckin { get; set; }
    }

    public class ElgSummaryReport
    {
        public decimal PosId { get; set; }
        public string PosName { get; set; }
        public decimal KHCN { get; set; }
        public decimal KHDN { get; set; }
        public decimal MainCust { get; set; }
        public decimal CustGoWith { get; set; }
        public decimal TotalUsed { get; set; }
        public decimal TotalBuyMore { get; set; }
        public decimal TotalUsedBSV { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
