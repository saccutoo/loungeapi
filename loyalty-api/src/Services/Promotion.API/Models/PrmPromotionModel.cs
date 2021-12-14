using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class PrmPromotionModel : BaseModel
    {
        public decimal? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? IsRegisterIAT { get; set; }
        public decimal? LimitOnPromotion { get; set; }
        public decimal? LimitOnCustomer { get; set; }
        public decimal? CumulativeValue { get; set; }
        public string ApprovedComment { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string StatusName { get; set; }
    }
    public class PrmPromotionCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? IsRegisterIAT { get; set; }
        public decimal? LimitOnPromotion { get; set; }
        public decimal? LimitOnCustomer { get; set; }
        public decimal? CumulativeValue { get; set; }
        public string CreateBy { get; set; }
        public string CreateByPos { get; set; }
        public string CreateByDept { get; set; }

    }
    public class PrmPromotionUpdateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? IsRegisterIAT { get; set; }
        public decimal? LimitOnPromotion { get; set; }
        public decimal? LimitOnCustomer { get; set; }
        public decimal? CumulativeValue { get; set; }
        public string Status { get; set; }
        public string LastModifiedBy { get; set; }
    }
    public class PrmPromotionApproveModel
    {
        public string ApprovedComment { get; set; }
        public string ApprovedBy { get; set; }
    }
    public class PrmPromotionQueryModel : PaginationRequest
    {
        public string Status { get; set; }
        public decimal? IsRegisterIAT { get; set; }
        public string DeptId { get; set; }
    }

    #region Core API
    public class AcctTideInfoModel
    {
        public string License { get; set; }
        public string Acct_No { get; set; }
        public string Cif_No { get; set; }
        public string Pos_CD { get; set; }
        public string Pos_Desc { get; set; }
        public string Prod_CD { get; set; }
        public string Prod_Desc { get; set; }
        public decimal Avail_Bal { get; set; }
        public decimal Curr_Mat_Amt { get; set; }
        public string Deposit_No { get; set; }
        public decimal Int_Mat { get; set; }
        public decimal Tenure { get; set; }
        public string Unit_Tenure { get; set; }
        public decimal Int_Rate { get; set; }
        public DateTime? Val_Dt { get; set; }
        public DateTime? Mat_Dt { get; set; }
        public DateTime? Cls_Dt { get; set; }
        public string Customer_Type { get; set; }
        public string Sex_Cd { get; set; }
        public DateTime? Birth_Day { get; set; }
        public string F_Name { get; set; }
        public string M_Name { get; set; }
        public string L_Name { get; set; }
        public string LL_Name { get; set; }
        public string Acct_Status { get; set; }
        public string Status { get; set; }
        public string Pos_Close { get; set; }
    }
    public class ResponseAcctTideInfoModel : ResponseCommonModel
    {
        public AcctTideInfoModel Data { get; set; }
    }
    public class ResponseListAcctTideInfoModel : ResponseCommonModel
    {
        public List<AcctTideInfoModel> Data { get; set; }
    }
    public class ResponseListCustClassInfoModel : ResponseCommonModel
    {
        public List<CustomerClass> Data { get; set; }
    }
    public class ResponseCommonModel
    {
        public StatusCode StatusCode { get; set; } = StatusCode.Success;
        public string Message { get; set; } = "Thành công";
        public decimal TotalCount { get; set; } = 0;
        public decimal TotalPage { get; set; } = 0;
        public decimal TotalRecord { get; set; } = 0;
    }
    public class CustomerClass
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    #endregion

    #region Util API
    public class PrmCustomerModel
    {
        public decimal ID { get; set; }
        public decimal UPLOADTRANSACTIONID { get; set; }
        public string CIFNUMBER { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string CUSTOMERCLASS { get; set; }
        public string CUSTOMERSTATUS { get; set; }
    }

    public class ResponsePrmCustomerModel : ResponseCommonModel
    {
        public List<PrmCustomerModel> Data { get; set; }
    }
    #endregion

    public class PrmPromotionViewModel
    {
        public string Branch { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public string CifNo { get; set; }
        public string DepositNo { get; set; }
        public string LegacyRefNo { get; set; }
        public decimal DepositVal { get; set; }
        public decimal Tenure { get; set; }
        public string ValidDate { get; set; }
        public string DueDate { get; set; }
        public string License { get; set; }
        public string Phone { get; set; }
        public string TaiKhoanTrungGianDVKH { get; set; }
        public string ApprovedComment { get; set; }
        public List<PrmProductInstanceModel> ListValidProduct { get; set; }
        public List<GiftInDebtModel> ListGiftInDebt { get; set; }
    }
}
