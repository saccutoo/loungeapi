using System;
using System.Runtime.Serialization;

namespace Utils
{
    public class EVoucherBaseModel
    {
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyBy { get; set; }
    }
}