using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    public class PrmProductInstanceModel
    {
        public decimal Id { get; set; }
        public decimal PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionCode { get; set; }
        public decimal ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductChannel { get; set; }
        public string PromotionForm { get; set; }
        public decimal LimitOnCustomer { get; set; } = 0;
        public string GiftValue { get; set; }
        public string SpendForm { get; set; }
        public string BranchScope { get; set; }
        public decimal IsNewCustomer { get; set; }
        public decimal IdentityCustomerUpload { get; set; }
        public string NotificationChannel { get; set; }
        public string NotificationImage { get; set; }
        public string NotificationLink { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public List<string> ListSpendForm { get; set; }
        public decimal GiftCashValue { get; set; }
        public decimal GiftCashValueReceived { get; set; }
        public string Gender { get; set; }
        public string CustClass { get; set; }
        public PrmPromotionModel PromotionInfo { get; set; }
        public List<PrmTransactionConditionModel> ListTransactionCondition { get; set; }
        public List<PrmGiftModel> ListGift { get; set; }
        public List<PrmOtherConditionModel> ListOtherCondition { get; set; }
        public List<PrmGiftModel> ListValidGift { get; set; }
        // Dùng khi sửa giao dịch bị từ chối để gửi duyệt lại
        public List<PrmTransactionLogDetailModel> ListPrmTransactionLogDetailModel { get; set; }
    }
    public class PrmProductInstanceCreateModel
    {
        public decimal PromotionId { get; set; }
        public decimal ProductId { get; set; }
        public decimal ProductCopyId { get; set; }
        public string PromotionForm { get; set; }
        public decimal LimitOnCustomer { get; set; } = 0;
        public string GiftValue { get; set; }
        public string SpendForm { get; set; }
        public string BranchScope { get; set; }
        public decimal IsNewCustomer { get; set; }
        public decimal IdentityCustomerUpload { get; set; }
        public string NotificationChannel { get; set; }
        public string NotificationImage { get; set; }
        public string NotificationLink { get; set; }
        public string Gender { get; set; }
        public string CustClass { get; set; }
        public string CreateBy { get; set; }
        public List<PrmTransactionConditionCreateModel> ListTransactionCondition { get; set; }
        public List<PrmGiftCreateModel> ListGift { get; set; }
        public List<PrmOtherConditionCreateModel> ListOtherCondition { get; set; }
        
    }
    public class PrmProductInstanceUpdateModel
    {
        public decimal PromotionId { get; set; }
        public decimal ProductId { get; set; }
        public decimal ProductCopyId { get; set; }
        public string PromotionForm { get; set; }
        public decimal LimitOnCustomer { get; set; }
        public string GiftValue { get; set; }
        public string SpendForm { get; set; }
        public string BranchScope { get; set; }
        public decimal IsNewCustomer { get; set; }
        public decimal IdentityCustomerUpload { get; set; }
        public string NotificationChannel { get; set; }
        public string NotificationImage { get; set; }
        public string NotificationLink { get; set; }
        public string LastModifiedBy { get; set; }
        public string Gender { get; set; }
        public string CustClass { get; set; }
        public List<PrmTransactionConditionCreateModel> ListTransactionCondition { get; set; }
        public List<PrmGiftCreateModel> ListGift { get; set; }
        public List<PrmOtherConditionCreateModel> ListOtherCondition { get; set; }
    }

    public class PrmProductInstanceQueryModel : PaginationRequest
    {
        
    }
}
