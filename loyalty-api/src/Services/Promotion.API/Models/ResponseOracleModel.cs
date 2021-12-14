using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher.API.Models
{
    public class ResponseOracleModel
    {
        public decimal Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }
    }
}
