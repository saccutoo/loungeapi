using System;
using System.Runtime.Serialization;

namespace Utils
{
    public class ELoungeBaseModel
    {
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}