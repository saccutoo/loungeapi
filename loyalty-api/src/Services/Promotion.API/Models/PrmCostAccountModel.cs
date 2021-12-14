using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    public class PrmCostAccountModel : BaseModel
    {
        public decimal? Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string GiftForm { get; set; }
        public string CustomerType { get; set; }
        public decimal IsRegisterIAT { get; set; }
        public string Description { get; set; }
        public string StatusName { get; set; }

    }
    public class PrmCostAccountCreateModel
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string GiftForm { get; set; }
        public string CustomerType { get; set; }
        public decimal IsRegisterIAT { get; set; }
        public string Description { get; set; }
        public decimal OrderView { get; set; }
        public string CreatedBy { get; set; }
    }
    public class PrmCostAccountUpdateModel
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string GiftForm { get; set; }
        public string CustomerType { get; set; }
        public decimal IsRegisterIAT { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal OrderView { get; set; }
        public string LastModifiedBy { get; set; }
    }

    public class PrmCostAccountQueryModel : PaginationRequest
    {
        public string Status { get; set; }
        public decimal? IsRegisterIAT { get; set; }
        public string GiftForm { get; set; }
        public string CustomerType { get; set; }
    }
}
