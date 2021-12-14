using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure.Migrations
{
    public class ElgLounge
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Logitude { get; set; }
        public int OrderView { get; set; }
    }
}
