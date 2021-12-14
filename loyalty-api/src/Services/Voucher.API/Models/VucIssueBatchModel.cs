using System;
using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucIssueBatchBaseModel : EVoucherBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Status { get; set; }
    }

    public class VucIssueBatchCreateUpdateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    public class VucIssueBatchQueryModel : PaginationRequest
    {

    }
}
