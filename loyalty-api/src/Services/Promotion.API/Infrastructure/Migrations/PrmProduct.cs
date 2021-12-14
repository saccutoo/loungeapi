using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class PrmProduct
    {
        public decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }       
        public string Channel { get; set; }
        public string CustomerType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public decimal OrderView { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal? TotalPage { get; set; }
        public decimal? TotalRecord { get; set; }
    }
}
