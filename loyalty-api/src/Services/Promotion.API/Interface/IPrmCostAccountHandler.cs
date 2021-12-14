using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IPrmCostAccountHandler
    {
        Task<Response> GetByFilterAsync(PrmCostAccountQueryModel queryModel);
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> CreateAsync(PrmCostAccountCreateModel model);
        Task<Response> UpdateAsync(decimal id, PrmCostAccountUpdateModel model);
        Task<Response> DeleteByIdAsync(decimal id);
    }
}
