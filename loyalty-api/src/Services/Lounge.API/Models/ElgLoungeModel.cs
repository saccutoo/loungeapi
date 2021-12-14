using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgLoungesBaseModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Logitude { get; set; }
        public int OrderView { get; set; }
    }

    public class ElgLoungesQueryModel : PaginationRequest
    {

    }
}
