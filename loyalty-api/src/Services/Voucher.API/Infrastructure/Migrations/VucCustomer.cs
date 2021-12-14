using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher.API.Infrastructure.Migrations
{
    public class VucCustomer
    {
        public decimal Id { get; set; }
        public decimal UploadTransactionId { get; set; }
        public string CIFNumber { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
