using System;
using System.Collections.Generic;
using Utils;

namespace Voucher.API.Models
{
    public class VucCustomerModel
    {
        public decimal Id { get; set; }
        public decimal UploadTransactionId { get; set; }
        public string CIFNumber { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }
    }

    public class VucCustomerQueryModel : PaginationRequest
    {
        public decimal? TransactionId { get; set; }
    }
}
