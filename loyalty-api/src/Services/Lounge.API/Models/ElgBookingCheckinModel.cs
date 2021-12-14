using Lounge.Models;
using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{

    public class ElgBookingCheckinModel : ElgBookingBaseModel
    {
        public decimal Elg_Cust_Id { get; set; }
        public decimal BookingId { get; set; }
        public string CustId { get; set; }
        public string CustName { get; set; }
        public string LoungeId { get; set; }
        public string FlightTiketNum { get; set; }
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
        public string ReservationCode { get; set; }
        public string VoucherId { get; set; }
        public DateTime CheckinTime { get; set; }
        public string CheckinNote { get; set; }
        public decimal BookingsStatusId { get; set; }
        public string Status { get; set; }
        public string OrderView { get; set; }
        public decimal IsAddBehavior { get; set; }
        public decimal TotalRecord { get; set; }
        public decimal TotalPage { get; set; }

    }

    public class ElgCheckInQueryModel : PaginationRequest
    {
        public string CusName { get; set; }
        public string FlightTikeNum { get; set; }
        public string CusId { get; set; }

    }
}