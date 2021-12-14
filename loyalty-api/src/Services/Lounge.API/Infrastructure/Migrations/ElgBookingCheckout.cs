using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class ElgBookingCheckout
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
        public string ImageUrl { get; set; }
        public decimal LoungeId { get; set; }
        public decimal NumOfPeopleGoWith { get; set; }
        public string ReservationCode { get; set; }
        public decimal Voucherid { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string PhoneNum { get; set; }
        public string RepresentUserName { get; set; }
        public string EloungeName { get; set; }
        public string EloungeAddress { get; set; }
        public string VoucherName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string BSVClassName { get; set; }
        public string BSVCardNum { get; set; }
        public decimal BuyMoreSlotQuantity { get; set; }
        public decimal BuyMoreSlotUnitPrice { get; set; }
        public decimal BuyMoreSlotTotal { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
