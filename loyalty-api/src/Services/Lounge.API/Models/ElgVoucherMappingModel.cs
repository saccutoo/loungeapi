using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgVoucherMappingBaseModel : ELoungeBaseModel
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
        public decimal AllowPrivateRoom { get; set; }
    }

    public class ElgVoucherMappingModel
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
    }

    public class ElgVoucherMapingCreateUpdateModel
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
    }
    public class ElgVoucherMappingQueryModel : PaginationRequest
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


    public class VucMappingVoucherCustModel
    {
        public decimal VoucherId { get; set; }
        public string TransType { get; set; }
        public string UploadId { get; set; }
        public List<CustVoucherModel> ListCustomer { get; set; }
    }

    public class VucMappingManualVoucherCustModel
    {
        public decimal VoucherId { get; set; }        
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CIF { get; set; }
        public string PhoneNumber { get; set; }
        public string Pos { get; set; }
        public string PosName { get; set; }
        public string RefNum { get; set; }
        public string ProgramName { get; set; }
        public decimal MaxUsedPerCust { get; set; }
        public DateTime ExpiredDate { get; set; }
        public decimal ClassId { get; set; }
        public decimal CustTypeId { get; set; }
        public string Email { get; set; }
    }

    public class CustVoucherModel
    {
        public string CustomerId { get; set; }
        public decimal VouCustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CIF { get; set; }
        public string Pos { get; set; }
        public decimal MaxUsedPerCust { get; set; }
    }


    public class VucMapVoucherCustQueryModel : PaginationRequest
    {
        public string TransType { get; set; }
        public decimal VoucherId { get; set; }
        public decimal ChannelId { get; set; }
        public decimal IssueBatchId { get; set; }
        public string Pos { get; set; }
        public decimal VoucherTypeId { get; set; }
    }
    public class VucMapVoucherCust
    {
        public decimal? Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Pos { get; set; }
        public decimal MaxUsedQuantityPerCust { get; set; }
        public decimal CountUsed { get; set; }
        public decimal PublishVoucherId { get; set; }
        public string SerialNum { get; set; }
        public string Status { get; set; }
        public string CustStatus { get; set; }
        public decimal? CustVouId { get; set; }
        public string TransType { get; set; }
        public decimal UploadId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Name { get; set; }
        public string ChannelName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string PosName { get; set; }
        public string RefNum { get; set; }
        public string ProgramName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }


    public class ElgVoucherSendMailModel
    {
        public decimal CustId { get; set; }
        public string Email { get; set; }
        public DateTime ExpireDate { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public string PhoneNum { get; set; }
        public decimal PosId { get; set; }
        public string PosName { get; set; }
        public string RepresentUserId { get; set; }
        public string RepresentUserName { get; set; }
        public DateTime VucExpiredDate { get; set; }
        public string VucName { get; set; }
        public string VucSerial { get; set; }
    }

    public class ElgCustInCoreModel
    {
        public string CustId { get; set; } //CUSTID
        public string Email { get; set; } //EMAIL
        public string FirstName { get; set; } 
        public string MiddleName { get; set; } 
        public string LastName { get; set; }
        public string PhoneNum { get; set; }
        public string FullName {
            get {
                var fullName = !string.IsNullOrEmpty(FirstName) ? FirstName + " " : string.Empty;
                fullName += !string.IsNullOrEmpty(MiddleName) ? MiddleName + " " : string.Empty;
                fullName += !string.IsNullOrEmpty(LastName) ? LastName: string.Empty;
                return fullName;
            }
        } //FullName

    }
}
