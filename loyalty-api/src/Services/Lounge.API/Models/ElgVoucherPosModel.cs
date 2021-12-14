using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Lounge.API.Models
{

    public class ElgVoucherPosModel
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

    public class ElgVoucherPosCreateModel
    {
        public string Pos { get; set; }
        public string Vuc_Code { get; set; }
        public decimal Quantity { get; set; }
        public decimal UploadTransactionId { get; set; }
        public decimal Quantity_Used { get; set; }
        public string CreateByUser { get; set; }
    }

    public class ElgVoucherPosQueryModel : PaginationRequest
    {
        public string Status { get; set; }
        public string Pos { get; set; }
        public string Vuc_Code { get; set; }
    }

    public class ElgVoucherPosChangeStausModel
    {
        public List<decimal> Ids { get; set; }
        public decimal UploadTransactionId { get; set; }
        public string UserAction { get; set; }
        public string Status { get; set; }

    }

    public class ElgVoucherPosStausConst
    {
        public const string WAITING_APPROVE = "WAITING_APPROVE";
        public const string REJECTED = "REJECTED";
        public const string APPROVED = "APPROVED";

    }

}
