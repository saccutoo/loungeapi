using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    public class ElgEmailBookingModel 
    {
        public string Email { get; set; }
        public string UserFullName { get; set; }
        public string ReservationCode { get; set; }
        public string PersonGoWith { get; set; }
        public string PhoneNumber { get; set; }
        public string BookingTime { get; set; }
    }   
}
