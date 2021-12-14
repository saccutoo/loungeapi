using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface ICriteriaConditionHandler
    {
        Task<Response> GetAllCriteriaActive();
        Task<Response> GetConditionByCriteria(string criteriaCode);
    }
}
