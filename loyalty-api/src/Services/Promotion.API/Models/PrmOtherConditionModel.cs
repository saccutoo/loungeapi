using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    public class PrmOtherConditionModel
    {
        public decimal Id { get; set; }
        public decimal ProductionInstanceId { get; set; }
        public string Criteria { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
    }
    public class PrmOtherConditionCreateModel
    {
        public string Criteria { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
    }  
    public class PrmOtherConditionQueryModel : PaginationRequest
    {
        
    }
}
