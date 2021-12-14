using Utils;

namespace API.Models
{
    #region Criteria
    public class CriteriaModel
    {
        public decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
    }
    public class CriteriaQueryModel : PaginationRequest
    {

    }
    #endregion

    #region Condition
    public class ConditionModel
    {
        public decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal OrderView { get; set; }
    }
    public class ConditionQueryModel : PaginationRequest
    {

    }
    #endregion

    #region Criteria Condition relationship
    public class CriteriaConditionModel
    {
        public decimal Id { get; set; }
        public string CriteriaCode { get; set; }
        public string ConditionCode { get; set; }
        public string Status { get; set; }
    }
    public class CriteriaConditionQueryModel : PaginationRequest
    {

    }
    #endregion
}
