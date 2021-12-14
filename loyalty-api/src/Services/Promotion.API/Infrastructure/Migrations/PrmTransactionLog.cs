using System;
using System.Collections.Generic;

namespace API.Infrastructure.Migrations
{
    public class PrmTransactionLog
    {
        public decimal Id { get; set; }
        public string Pos { get; set; }
        public string PosDesc { get; set; }
        public string UserStaff { get; set; }
        public string CifNumber { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string SaveAccountNumber { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal DepositTerm { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public decimal IsInDebt { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public decimal DepositValue { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedComment { get; set; }
        public string License { get; set; }
        public string Phone { get; set; }
        public DateTime RevokedDate { get; set; }
        public string RevokedBy { get; set; }
        public string RevokedComment { get; set; }
        public string UserStaffRevoke { get; set; }
        public string PosRevoke { get; set; }
        public string GiftInDebt { get; set; }
        public string AccountPayTax { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
    public class PrmTransactionLogDetail
    {
        public decimal Id { get; set; }
        public decimal ProductInstanceId { get; set; }
        public decimal TransactionId { get; set; }
        public decimal PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public string SpendForm { get; set; }
        public string GiftForm { get; set; }
        public string GiftName { get; set; }
        public decimal Quantity { get; set; }
        public decimal GiftValue { get; set; }
        public decimal GiftCashValue { get; set; }
        public string Status { get; set; }
        public string RefNumber { get; set; }
        public string IasCodeReference { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string TransactionType { get; set; }
        public string PosCd { get; set; }
        public string PostingResultCode { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }

    public class AccountingDetail
    {
        public decimal LogDetailId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public string FromAccountNumber { get; set; }
        public string FromAccountName { get; set; }
        public string ToAccountNumber { get; set; }
        public string CifNumber { get; set; }
        public string CustomerName { get; set; }
        public string RefNumber { get; set; }
        public string GiftName { get; set; }
        public decimal Quantity { get; set; }
        public decimal GiftValue { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string IasCodeReference { get; set; }
        public string GiftForm { get; set; }
        public string PosCd { get; set; }
        public decimal IasTransHeadId { get; set; }
        public string AccountPayTax { get; set; }
        public string SaveAccountNumber { get; set; }
        public string Pos { get; set; }
    }

    public class PrmFinancialPosting
    {
        public decimal Id { get; set; }
        public decimal TransactionId { get; set; }
        public decimal TransactionDetailId { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string RefNo { get; set; }
        public string PostingResultCode { get; set; }
        public string PostingResultDesc { get; set; }
        public string Remarks { get; set; }
    }
}
