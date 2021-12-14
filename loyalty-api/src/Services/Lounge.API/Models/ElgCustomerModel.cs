using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgCustomerBaseModel : ELoungeBaseModel
    {
        public string Position { get; set; }
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
        public string RepresentUserId { get; set; }
        public string RepresentUserName { get; set; }
        public decimal UPLOADTRANSACTIONID { get; set; }
        public string CustClassName { get; set; }
        public string CustTypeName { get; set; }
        public string BranchName { get; set; }
        public decimal MaxPeopleGoWith { get; set; }
        public decimal MaxOfUse { get; set; }
        public decimal MaxCheckin { get; set; }
        public string PosName { get; set; }
        public decimal[] MapVoucherId { get; set; }
        public string[] VoucherName { get; set; }
        public decimal[] MaxUsedQuantityPerCust { get; set; }
        public decimal[] CountUsed { get; set; }

        public decimal ItemMaxUsedQuantityPercust { get; set; }
        public decimal ItemCountUsed { get; set; }
        public decimal ItemMapVoucherId { get; set; }
        public string ItemVoucherName { get; set; }
        public decimal AllowPrivateRoom { get; set; }
        public string TypeUpdate { get; set; }

    }

    public class ElgCustomerCreateUpdateModel
    {
        public decimal PosId { get; set; }
        public string PosName { get; set; }
        public decimal CustTypeId { get; set; }
        public decimal ClassId { get; set; }
        public decimal UploadId { get; set; }
        public string CustId { get; set; }
        public string CifNum { get; set; }
        public string FullName { get; set; }
        public string RepresentUserName { get; set; }
        public string RepresentUserId { get; set; }
        public DateTime BirthDay { get; set; }
        public string Email { get; set; }
        public string PhoneNum { get; set; }
        public string Gender { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Position { get; set; }

    }

    public class ElgCustomerResolveConflictModel
    {
        public string CustId { get; set; }
        public string CifNum { get; set; }
        public string Email { get; set; }
        public string PhoneNum { get; set; }
    }

    public class ElgCustomerQueryModel : PaginationRequest
    {
        public decimal UPLOADTRANSACTIONID { get; set; }
        public decimal PosId { get; set; }
        public string CustId { get; set; }
        public decimal CustTypeId { get; set; }
        public decimal ClassId { get; set; }
        public string CifNum { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Status { get; set; }
    }

    public class ElgCustomerExportModel
    {
        public string FullTextSearch { get; set; }
        public decimal UploadId { get; set; }
        public decimal PosId { get; set; }
        public string CifNum { get; set; }
        public decimal CustTypeId { get; set; }
        public decimal CustClassId { get; set; }
        public decimal CustId { get; set; }
        public decimal StatusId { get; set; }
        public DateTime ExpireDate { get; set; }
    }
    public class ElgCustomerApproveModel
    {
        //public decimal PosId { get; set; }
        //public string PosName { get; set; }
        //public decimal CustTypeId { get; set; }
        //public decimal ClassId { get; set; }
        //public decimal UploadId { get; set; }
        public string CustId { get; set; }
        //public string CifNum { get; set; }
        //public string FullName { get; set; }
        //public string RepresentUserName { get; set; }
        //public string RepresentUserId { get; set; }
        //public DateTime BirthDay { get; set; }
        //public string Email { get; set; }
        //public string PhoneNum { get; set; }
        //public DateTime ExpireDate { get; set; }
    }

    public class ElgCustomerDecryptModel
    {
        public decimal Id { get; set; }
        public string FullName { get; set; }
        public string CustId { get; set; }
        public string PhoneNum { get; set; }
        public string RepresentUserName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
    }
}
