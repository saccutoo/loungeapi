using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    public class ElgReportBaseModel : ELoungeBaseModel
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
        public decimal UploadId { get; set; }
        public string CustClassName { get; set; }
        public string CustTypeName { get; set; }
        public string BranchName { get; set; }
        public string PosName { get; set; }
        public decimal NumOfUse { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public DateTime BookingTime { get; set; }
        public string GoWithName { get; set; }
        public string GoWithPhoneNum { get; set; }
        public string GoWithCustId { get; set; }
    }

    public class ElgReportCustomerModel : ELoungeBaseModel
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
        public decimal UploadId { get; set; }
        public string CustClassName { get; set; }
        public string CustTypeName { get; set; }
        public string BranchName { get; set; }
        public string PosName { get; set; }
        public decimal NumOfUse { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public DateTime BookingTime { get; set; }
        public string GoWithName { get; set; }
        public string GoWithPhoneNum { get; set; }
        public DateTime? LastCheckin { get; set; }
    }

    public class ElgReportBookingModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string CustId { get; set; }
        public string CifNum { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDay { get; set; }
        public string PhoneNum { get; set; }
        public string RepresentUserName { get; set; }
        public string Email { get; set; }
        public decimal PosId { get; set; }
        public string PosName { get; set; }
        public decimal NumOfUse { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public decimal CustTypeId { get; set; }
        public string CustClassName { get; set; }
        public DateTime BookingTime { get; set; }
        public string GoWithName { get; set; }
        public string GoWithPhoneNum { get; set; }
        public string GoWithCustId { get; set; }
        public string CustTypeName { get; set; }
        public decimal TotalBuyMore { get; set; }
        public decimal TotalUsedBSV { get; set; }
        public string BSVClassName { get; set; }
    }
    public class ElgExportDashboardModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string BookingNote { get; set; }
        public decimal BookingStatusId { get; set; }
        public DateTime BookingTime { get; set; }
        public string CheckedInNote { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string CustId { get; set; }
        public string CustName { get; set; }
        public string FlightTiketNum { get; set; }
        public string ImageUrl { get; set; }
        public decimal LoungeId { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public string ReservationCode { get; set; }
        public decimal Voucherid { get; set; }
        public string VoucherName { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string PhoneNum { get; set; }
        public string RepresentUserName { get; set; }
        public string EloungeName { get; set; }
        public string EloungeAddress { get; set; }
        public string BSVClassName { get; set; }
        public string BSVCardNum { get; set; }
        public decimal BuyMoreSlotQuantity { get; set; }
        public decimal BuyMoreSlotUnitPrice { get; set; }
        public decimal BuyMoreSlotTotal { get; set; }
        public string BuyMoreSlot { get; set; }
        public DateTime Birthday { get; set; }
        public string Position { get; set; }

    }
    public class ElgReportQueryModel : PaginationRequest
    {
        public decimal CustTypeId { get; set; }
        public decimal ClassId { get; set; }
        public decimal PosId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CustIds { get; set; }
        public string VoucherUse { get; set; }
    }
    public class ElgSummaryReportModel
    {
        public decimal PosId { get; set; }
        public string PosName { get; set; }
        public decimal KHCN { get; set; }
        public decimal KHDN { get; set; }
        public decimal MainCust { get; set; }
        public decimal CustGoWith { get; set; }
        public decimal TotalUsed { get; set; }
        public decimal TotalBuyMore { get; set; }
        public decimal TotalUsedBSV { get; set; }
    }

    public class ElgCheckinPeopleGoWidthBaseModel : ELoungeBaseModel
    {
        public decimal Id { get; set; }
        public string CustId { get; set; }
        public string CustName { get; set; }
        public string PhoneNum { get; set; }
        public string FlightTicketNum { get; set; }
        public string BookingId { get; set; }
        public string Status { get; set; }
        public string Gender { get; set; }
        public decimal IsCustomerSHB { get; set; }
        public DateTime BirthDay { get; set; }
        public string CusNameBooking { get; set; }
        public string RepreSentUserName { get; set; }
        public decimal TotalRecord { get; set; }
        public decimal TotalPage { get; set; }
    }
    public class ElgCheckinPeopleGoWidthQueryModel : PaginationRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
