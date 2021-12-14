using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgCustomerInfoModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public DateTime BirthDay { get; set; }
        public string CifNum { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string CustId { get; set; }
        public string Gender { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string PhoneNum { get; set; }
        public string Avatar { get; set; }
        public decimal AllowPrivateRoom { get; set; }
        public string Position { get; set; }

    }

    public class ElgCustomerInfoUpdateModel
    {
        public DateTime BirthDay { get; set; }
        public string CifNum { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string CustId { get; set; }
        public string Gender { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string PhoneNum { get; set; }
        public string Avatar { get; set; }
        public decimal AllowPrivateRoom { get; set; }
        public string Position { get; set; }

    }

    public class ElgCustomerInfoQueryModel : PaginationRequest
    {

    }  
}
