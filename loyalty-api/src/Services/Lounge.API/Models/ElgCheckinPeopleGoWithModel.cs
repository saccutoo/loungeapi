using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgCheckinPeopleGoWithBaseModel
    {
        public decimal? Id { get; set; }
        public decimal BookingId { get; set; }
        public string Status { get; set; }
        public string CustId { get; set; }
        public string CustName { get; set; }
        public string FlightTicketNum { get; set; }
        public string PhoneNum { get; set; }
        public string Gender { get; set; }
        public string IsCustomerShb { get; set; }
        public DateTime BirthDay { get; set; }

    }

    public class ElgCheckinPeopleGoWithModel
    {
        public string CustId { get; set; }
        public string CustName { get; set; }
        public string PhoneNum { get; set; }
        public string FlightTicketNum { get; set; }
        public string Gender { get; set; }
        public decimal IsCustomerSHB { get; set; }
        public DateTime BirthDay { get; set; }
    }

    public class ElgCheckinPeopleGoWithCheckinModel
    {
        public decimal ElgCustId { get; set; }
        public decimal BookingId { get; set; }
        public string CustId { get; set; }
        public string FlightTicketNum { get; set; }
        public string FlightTimeFrom { get; set; }
        public string FlightTimeTo { get; set; }
        public string FlightLocationFrom { get; set; }
        public string FlightLocationTo { get; set; }
        public decimal MapVoucherId { get; set; }
        public string SerialVoucher { get; set; }
        public string BookingNote { get; set; }
        public string ImageUrl { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public string BSVClassName { get; set; }
        public string BSVCardNum { get; set; }
        public decimal BuyMoreSlotQuantity { get; set; }
        public decimal BuyMoreSlotUnitPrice { get; set; }
        public decimal BuyMoreSlotTotal { get; set; }
        public DateTime BookingTime { get; set; }
        public List<ElgCheckinPeopleGoWithModel> lstPeopleGoWith { get; set; }
        public string CustIdNew { get; set; }
        public string FaceId { get; set; }

    }

    public class ElgCheckinPeopleGoWithQueryModel : PaginationRequest
    {
        public string Status { get; set; }
    }
    public class GetDashboardCheckInOut 
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; }
    }
}
