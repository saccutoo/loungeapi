using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    public class PrmTransactionConditionModel
    {
        public decimal Id { get; set; }
        public decimal ProductionInstanceId { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal FromTerm { get; set; }
        public decimal ToTerm { get; set; }
        public decimal CashValue { get; set; }
        public decimal PercentValue { get; set; }
    }
    public class PrmTransactionConditionCreateModel
    {
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal FromTerm { get; set; }
        public decimal ToTerm { get; set; }
        public decimal CashValue { get; set; }
        public decimal PercentValue { get; set; }
    }  
    public class PrmTransactionConditionQueryModel : PaginationRequest
    {
        
    }
}
