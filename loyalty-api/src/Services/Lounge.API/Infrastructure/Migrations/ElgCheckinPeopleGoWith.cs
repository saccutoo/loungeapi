using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class ElgCheckinPeopleGoWith
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
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
