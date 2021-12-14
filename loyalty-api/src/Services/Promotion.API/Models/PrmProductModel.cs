using Utils;

namespace API.Models
{
    public class PrmProductModel : BaseModel
    {
        public decimal? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public string CustomerType { get; set; }
        public string Description { get; set; }
        public string StatusName { get; set; }
    }
    public class PrmProductCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public string CustomerType { get; set; }
        public string Description { get; set; }
        public decimal OrderView { get; set; }
        public string CreateBy { get; set; }
    }
    public class PrmProductUpdateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public string CustomerType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal OrderView { get; set; }
        public string LastModifiedBy { get; set; }
    }

    public class PrmProductQueryModel : PaginationRequest
    {
        public string Status { get; set; }
        public string Channel { get; set; }
    }
}
