using System;
using System.Runtime.Serialization;

namespace Utils
{
    public class BaseModel
    {
        public string Status { get; set; }
        public decimal OrderView { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}