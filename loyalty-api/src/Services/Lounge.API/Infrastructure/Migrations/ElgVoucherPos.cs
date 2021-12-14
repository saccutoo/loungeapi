using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class ElgVoucherPos
    {
        public decimal Id { get; set; }
        public string Pos { get; set; }
        public string Vuc_Code { get; set; }
        public decimal Quantity { get; set; }
        public decimal UploadTransactionId { get; set; }
        public decimal Quantity_Used { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovedByUser { get; set; }
        public string CreateByUser { get; set; }
        public string Status { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
