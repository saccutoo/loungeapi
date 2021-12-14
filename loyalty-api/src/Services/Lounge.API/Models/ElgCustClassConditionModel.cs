using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgCustClassConditionBaseModel : ELoungeBaseModel
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
    }

    public class ElgCustClassConditionCreateUpdateModel
    {
        public decimal CustClassId { get; set; }
        public decimal MaxCheckin { get; set; }
        public decimal MaxPeopleGoWith { get; set; }
        public string ConditionUse { get; set; }
        public DateTime ExpireDate { get; set; }
        public int UnlimitExpireTime { get; set; }        
    }

    public class ElgCustClassConditionQueryModel : PaginationRequest
    {
        public decimal CustClassId { get; set; }
        public decimal MaxPeopleGoWith { get; set; }
        public decimal MaxOfUse { get; set; }
    }
}
