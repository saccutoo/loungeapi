using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgConfigurateBaseModel : ELoungeBaseModel
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }
    }

    public class ElgConfigurateCreateUpdateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
    }

    public class ElgConfigurateQueryModel : PaginationRequest
    {

    }
}
