using System;

namespace API.Infrastructure.Migrations
{
    public class ElgCustomerInfo
    {
        public decimal? Id { get; set; }
        public DateTime BirthDay { get; set; }
        public string CifNum { get; set; }
        public string Email { get; set; }
        public string CustId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string PhoneNum { get; set; }
        public string Avatar { get; set; }
        public string Position { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
        public decimal  AllowPrivateRoom { get; set; }
    }
}
