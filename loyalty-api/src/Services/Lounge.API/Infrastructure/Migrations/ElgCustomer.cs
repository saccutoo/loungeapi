using System;

namespace API.Infrastructure.Migrations
{
    public class ElgCustomer
    {
        public decimal? Id { get; set; }
        public DateTime BirthDay { get; set; }
        public decimal ClassId { get; set; }
        public string CustId { get; set; }
        public string CifNum { get; set; }
        public decimal CustTypeId { get; set; }
        public string Email { get; set; }
        public DateTime ExpireDate { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string PhoneNum { get; set; }
        public decimal PosId { get; set; }
        public string PosName { get; set; }
        public string Position { get; set; }
        public string RepresentUserId { get; set; }
        public string RepresentUserName { get; set; }
        public decimal UPLOADTRANSACTIONID { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
        public string CustClassName { get; set; }
        public string CustTypeName { get; set; }
        public string BranchName { get; set; }
        public decimal MaxPeopleGoWith { get; set; }
        public decimal MaxOfUse { get; set; }
        public decimal MaxCheckin { get; set; }
        public decimal ItemMaxUsedQuantityPercust { get; set; }
        public decimal ItemCountUsed { get; set; }
        public decimal ItemMapVoucherId { get; set; }
        public string ItemVoucherName { get; set; }

    }

    public class ElgKYCCustomer
    {
        public string CustId { get; set; }
        public decimal CustVoucherId { get; set; }
        public string CustVoucherName { get; set; }
        public decimal Cus_MaxUsedQuantity { get; set; }
        public decimal Cus_CountUsed { get; set; }
        public string Cus_Voucher_Status { get; set; }
        

        public string CustomerId { get; set; }
        public decimal MapVoucherId { get; set; }
        public string VoucherName { get; set; }
        public decimal Vuc_MaxUsedQuantity { get; set; }
        public decimal Vuc_CountUsed { get; set; }
    }

    public class ElgUploadId
    {
        public decimal UPLOADTRANSACTIONID { get; set; }
        public string UPLOADTRANSACTION { get; set; }
    }

    public class ElgUplTransaction
    {
        public decimal Id { get; set; }
        public decimal TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ApplicationCode { get; set; }
        public string ApplicationName { get; set; }
        public string ApproveComment { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Code { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
