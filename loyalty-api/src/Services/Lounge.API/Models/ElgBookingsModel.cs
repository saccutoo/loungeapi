using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgBookingsBaseModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string BookingNote { get; set; }
        public decimal BookingStatusId { get; set; }
        public DateTime BookingTime { get; set; }
        public string CheckedInNote { get; set; }
        public DateTime CheckInTime { get; set; }
        public string CustId { get; set; }
        public string CustName { get; set; }
        public string FlightTiketNum { get; set; }
        public string FlightTimeFrom { get; set; }
        public string FlightTimeTo { get; set; }
        public string FlightLocationFrom { get; set; }
        public string FlightLocationTo { get; set; }

        public string ImageUrl { get; set; }
        public decimal LoungeId { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public string ReservationCode { get; set; }
        public decimal Voucherid { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public decimal ClassId { get; set; }
        public string CustClassName { get; set; }
        public decimal UploadId { get; set; }
        public decimal CustTypeId { get; set; }
        public string CustTypeName { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal PosId { get; set; }
        public string PosName { get; set; }
        public string PhoneNum { get; set; }
        public DateTime BirthDay { get; set; }
        public string Gender { get; set; }
        public string RepresentUserName { get; set; }
        public string RepresentUserId { get; set; }
        public string Email { get; set; }
        public string CifNum { get; set; }
        public decimal MaxPeopleGoWith { get; set; }
        public decimal AllowPrivateRoom { get; set; }
        public decimal Elg_Cust_Id { get; set; }
    }
    public class ElgBookingsCreateUpdateModel
    {
        public decimal ElgCustId { get; set; }
        public string CustId { get; set; }
        public string CustName { get; set; }
        public decimal LoungeId { get; set; }
        public DateTime BookingTime { get; set; }
        public string ReservationCode { get; set; }
        public string BookingNote { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public decimal MapVoucherId { get; set; }
    }
    public class ElgBookingsQueryModel : PaginationRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ElgCheckBookingModel
    {
        public ElgCustomerBaseModel customerModel;
        public List<ElgCustomerBaseModel> listCustomerModel;
        public List<ElgBookingsBaseModel> listBookingModel;
        public List<ElgPartnerClass> LstElgPartnerClass { get; set; }
        public ElgFaceCustomerViewModel FaceCustomer { get; set; }

    }
    public class ElgPartnerClass
    {
        public string PartnerName { get; set; }
        public string CustomerClass { get; set; }
        public string PartnerCode { get; set; }
    }

    public class ElgBookingUpdateBehavior
    {
        public decimal BookingId { get; set; }
        public decimal IsAddBehavior { get; set; }

    }
}
