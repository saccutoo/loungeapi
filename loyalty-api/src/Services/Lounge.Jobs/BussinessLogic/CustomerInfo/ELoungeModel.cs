using System;
using System.Collections.Generic;
using Utils;

namespace API.BussinessLogic
{
    public class ElgMaxCustomer
    {
        public decimal MaxId { get; set; }
    }
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
    public class ElgCustomerBaseModel : ELoungeBaseModel
    {
        public decimal Id { get; set; }
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
        public string Bin { get; set; }
        public string ContractId { get; set; }
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

    public class ElgConfigurateBaseModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }
    }

    public class ElgConfigurateCreateUpdateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
    }


    public class ElgConfigurate
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Value { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }

    public class ElgConfigurateQueryModel : PaginationRequest
    {

    }

}
