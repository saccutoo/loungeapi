using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class ElgStaff
    {
        public decimal? Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public decimal LoungeId { get; set; }
        public string PersonalId { get; set; }
        public string PhoneNum { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public int? FailedPassCount { get; set; }
        public DateTime? LockTime { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string JWT { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
        public string LoungeName { get; set; }
        public string LoungeAddress { get; set; }
    }
}
