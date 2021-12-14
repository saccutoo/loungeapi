using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    #region Transaction Log
    public class PrmTransactionLogModel
    {
        public decimal Id { get; set; }
        public string Pos { get; set; }
        public string PosDesc { get; set; }
        public string DeptId { get; set; }
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
        public decimal TotalGiftValue { get; set; } = 0;
        public string TaiKhoanTrungGianDVKH { get; set; }
        public List<PrmProductTransactionLogModel> ListPrmProductTransactionLogModel { get; set; }
    }
    public class PrmTransactionLogCreateModel
    {
        public decimal Id { get; set; }
        public string Pos { get; set; }
        public string PosDesc { get; set; }
        public string DeptId { get; set; }
        public string UserStaff { get; set; }
        public string CifNumber { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string SaveAccountNumber { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal DepositTerm { get; set; }
        public decimal IsInDebt { get; set; }
        public decimal DepositValue { get; set; }
        public string Lincense { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public List<PrmProductTransactionLogCreateModel> ListPrmProductTransactionLogModel { get; set; }
        public List<GiftInDebtModel> ListGiftInDebt { get; set; }
        public string AccountPayTax { get; set; }
    }
    public class PrmTransactionLogQueryModel : PaginationRequest
    {
        public string UserStaff { get; set; }
        public string CifNumber { get; set; }
        public string SaveAccountNumber { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? ApprovedFrom { get; set; }
        public DateTime? ApprovedTo { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
        public string Pos { get; set; }
        public string DeptId { get; set; }
        public bool IsFilterRevoke { get; set; } = false;
        public bool IsFilterByPos { get; set; } = true;
    }
    #endregion

    #region Product Transaction Log
    public class PrmProductTransactionLogModel
    {
        public decimal GiftCashValue { get; set; }
        public decimal ProductInstanceId { get; set; }
        public decimal TransactionId { get; set; }
        public decimal PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public string SpendForm { get; set; }
        public List<PrmTransactionLogDetailModel> ListPrmTransactionLogDetailModel { get; set; }
    }
    public class PrmProductTransactionLogCreateModel
    {
        public decimal ProductInstanceId { get; set; }
        public decimal TransactionId { get; set; }
        public decimal PromotionId { get; set; }
        public string PromotionName { get; set; }
        public List<PrmTransactionLogDetailCreateModel> ListPrmTransactionLogDetailModel { get; set; }
    }

    public class GiftInDebtModel
    {
        public string PromotionCode { get; set; }
        public string GiftName { get; set; }
        public decimal Quantity { get; set; }
        public string IasCodeReference { get; set; }
    }
    #endregion

    #region Transaction Log Detail
    public class PrmGiftStockDetail
    {
        public decimal LogDteailId { get; set; }
        public decimal IasTransDetailId { get; set; }
        public string IasCodeReference { get; set; }
        public decimal DetailIdLink { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class PrmTransactionLogDetailModel
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
    }
    public class PrmTransactionLogDetailCreateModel
    {
        public string GiftForm { get; set; }
        public string GiftName { get; set; }
        public decimal Quantity { get; set; }
        public decimal GiftValue { get; set; }
        public decimal GiftCashValue { get; set; }
        public string RefNumber { get; set; }
        public string IasCodeReference { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string TransactionType { get; set; }
        public string PosCd { get; set; }
        public string Remarks { get; set; }
    }
    public class PrmTransactionLogDetailQueryModel : PaginationRequest
    {

    }
    #endregion

    #region Accounting Model
    public class AccountingModel
    {
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public List<AccountingDetailModel> ListAccountingDetailModel { get; set; }
    }
    public class AccountingDetailModel
    {
        public decimal LogDetailId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public string FromAccountNumber { get; set; }
        public string FromAccountName { get; set; }
        public string ToAccountNumber { get; set; }
        public string ToAccountName { get; set; }
        public string PosCd { get; set; }
        public string CifNumber { get; set; }
        public string RefNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccount { get; set; }
        public string GiftName { get; set; }
        public decimal Quantity { get; set; }
        public decimal GiftValue { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string IasCodeReference { get; set; }
        public string GiftForm { get; set; }
        public decimal IasTransHeadId { get; set; }
        public string AccountPayTax { get; set; }
        public string SaveAccountNumber { get; set; }
        public string Pos { get; set; }
        public FinancialPostingRequestModel FinancialPostingRequest { get; set; }
    }
    public class FinancialPostingRequestModel
    {
        public string fromAccount { get; set; }
        public decimal logDetailId { get; set; }
        public string channelCode { get; set; }
        public string transDate { get; set; }
        public string refNo { get; set; }
        public string sourceBranchCode { get; set; }
        public string postingFlag { get; set; } = "F";
        public string transBalanceFlag { get; set; } = "Y";
        public string postAllSegFlag { get; set; } = "N";
        public string numOfSeq { get; set; }
        public string checkSum { get; set; }
        public string remarks { get; set; }
        public decimal IasTransHeadId { get; set; }
        public List<SegInfos> listSegInfos { get; set; }
    }
    public class FinancialPostingResponseModel
    {
        public string fromAccount { get; set; }
        public string toAccount { get; set; }
        public decimal logDetailId { get; set; }
        public string errorCode { get; set; }
        public string errorDesc { get; set; }
        public string channelId { get; set; }
        public string headerRefNo { get; set; }
        public string transDate { get; set; }
        public string transStatus { get; set; }
        public string transResultCode { get; set; }
        public string refNo { get; set; }
        public string numOfSeq { get; set; }
        public string remarks { get; set; }
        public decimal IasTransHeadId { get; set; }
        public decimal Amount { get; set; }
    }
    public class SegInfos
    {
        public string segNo { get; set; }
        public string accountName { get; set; }
        public string accountNum { get; set; }
        public string accountBranchCode { get; set; }
        public string debitCreditFlag { get; set; }
        public string transAmount { get; set; }
        public string transCurrency { get; set; }
        public string extRemark { get; set; }
    }
    #endregion

    #region IAS
    public class IAS_ExportTransCreateModel
    {
        public List<IAS_NVLItemDetailModel> ItemRequest { get; set; }
        public decimal PromoId { get; set; }
        public string PosCd { get; set; }
        public string DeptCd { get; set; }
        public string UserName { get; set; }
    }
    public class IAS_NVLItemDetailModel
    {
        public decimal TRANS_DETAIL_ID { get; set; }
        public decimal ITEM_ID { get; set; }
        public string POS_CD { get; set; }
        public string DEPT_CD { get; set; }
        public decimal REQ_DETAIL_ID { get; set; }
        public decimal DETAIL_ID_LINK { get; set; }
        public decimal QTY_TRANS { get; set; }
        public double PRICE_NO_VAT { get; set; }
        public double PRICE { get; set; }
        public double VAT { get; set; }
        public double AMOUNT { get; set; }
    }
    public class IASTransHeadRes
    {
        public decimal TransHeadId { get; set; }
        public List<IASTransDetailRes> ListTransDetail { get; set; }
    }
    public class IASTransDetailRes
    {
        public decimal TransDetailId { get; set; }
        public decimal ItemId { get; set; }
        public decimal DetailIdLink { get; set; }
    }
    public class IASApproveTransHeadModel
    {
        public decimal TransHeadId { get; set; }
        public string RefNo { get; set; }
        public string ApprovedBy { get; set; }
    }
    #endregion

    #region Customer Account Info
    public class AccountModel
    {
        [JsonProperty("custId")]
        public string CustId { get; set; }
        [JsonProperty("custName")]
        public string CustName { get; set; }
        [JsonProperty("prodCD")]
        public string ProdCD { get; set; }
        [JsonProperty("prodDesc")]
        public string ProdDesc { get; set; }
        [JsonProperty("inactSt")]
        public string InactSt { get; set; }
        [JsonProperty("acctFinSts")]
        public string AcctFinSts { get; set; }
        [JsonProperty("accountNum")]
        public string AccountNum { get; set; }
        [JsonProperty("accountType")]
        public string AccountType { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("branchId")]
        public string BranchId { get; set; }
        [JsonProperty("branchname")]
        public string Branchname { get; set; }
        [JsonProperty("curBal")]
        public string CurBal { get; set; }
        [JsonProperty("availBal")]
        public string AvailBal { get; set; }
    }

    public class AccountQueryModel
    {
        [JsonProperty("accountType")]
        public string AccountType { get; set; }
        [JsonProperty("checkSum")]
        public string CheckSum { get; set; }
        [JsonProperty("custType")]
        public string CustType { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
    }

    public class ResponseAccountModel
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
        [JsonProperty("errorDesc")]
        public string ErrorDesc { get; set; }
        [JsonProperty("listAccounts")]
        public List<AccountModel> ListAccounts { get; set; }
    }
    #endregion
}
