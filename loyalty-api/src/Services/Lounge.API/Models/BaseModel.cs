using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lounge.Models
{
    public class BaseModel
    {
    }
    public class ElgBookingBaseModel
    {
        public decimal Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
